import * as React from 'react';
import * as XLSX from 'xlsx';
import {
  TableContainer,
  Table,
  TableRow,
  TableCell,
  TableBody,
  Typography,
  TableHead,
  Chip,
  Box,
  MenuItem,
  Button,
  Divider,
  IconButton,
  Grid,
  Stack,
  CircularProgress,
} from '@mui/material';
import DownloadCard from 'src/components/shared/DownloadCard';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import CustomSelect from 'src/components/forms/theme-elements/CustomSelect';
import {
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
  createColumnHelper,
} from '@tanstack/react-table';
import {
  IconChevronLeft,
  IconChevronRight,
  IconChevronsLeft,
  IconChevronsRight,
  IconEdit,
} from '@tabler/icons-react';
import EmptyImage from 'src/assets/images/svgs/no-data.webp';
import { useTranslation } from 'react-i18next';

import { useMasterData } from 'src/hooks/useMasterData';

const columnHelper = createColumnHelper();

function Filter({ column, t }) {
  const columnFilterValue = column.getFilterValue();
  const { filterVariant } = column.columnDef.meta || {};

  if (filterVariant === 'select') {
    return (
      <CustomSelect
        value={columnFilterValue ?? ''}
        onChange={(e) => column.setFilterValue(e.target.value)}
        fullWidth
        size="small"
      >
        <MenuItem value="">{t('Placeholder.AllStatus') || 'All Status'}</MenuItem>
        <MenuItem value="true">{t('Status.Active') || 'Active'}</MenuItem>
        <MenuItem value="false">{t('Status.Inactive') || 'Inactive'}</MenuItem>
      </CustomSelect>
    );
  }

  return (
    <CustomTextField
      placeholder={t('Placeholder.Search')}
      value={columnFilterValue || ''}
      onChange={(e) => column.setFilterValue(e.target.value)}
      fullWidth
      size="small"
    />
  );
}

const DepartmentListTable = ({ onEditClick }) => {
  const { t } = useTranslation();

  const { departments } = useMasterData();

  const data = React.useMemo(() => departments || [], [departments]);

  const loading = !departments;

  const [columnFilters, setColumnFilters] = React.useState([]);
  const [sorting, setSorting] = React.useState([]);

  const columns = React.useMemo(
    () => [
      columnHelper.accessor((row) => row?.name ?? '', {
        header: t('Field.Name'),
        size: 250,
      }),
      columnHelper.accessor((row) => row?.note ?? '', {
        header: t('Field.Note'),
        size: 300,
        cell: (info) => {
          const value = info.getValue();
          return (
            <Typography
              variant="body2"
              sx={{
                maxWidth: 280,
                whiteSpace: 'nowrap',
                overflow: 'hidden',
                textOverflow: 'ellipsis',
              }}
              dangerouslySetInnerHTML={{ __html: value || '-' }}
            />
          );
        },
      }),
      columnHelper.accessor((row) => row?.isActivated ?? false, {
        header: t('Field.Status'),
        size: 150,
        cell: (info) => (
          <Chip
            label={info.getValue() ? t('Status.Active') : t('Status.Inactive')}
            color={info.getValue() ? 'success' : 'error'}
            size="small"
          />
        ),
        meta: { filterVariant: 'select' },
      }),
      columnHelper.display({
        id: 'actions',
        header: '',
        size: 100,
        cell: (info) => (
          <Box sx={{ display: 'flex', justifyContent: 'center' }}>
            <Button
              color="primary"
              size="small"
              sx={{ minWidth: 0, padding: '6px' }}
              onClick={() => onEditClick?.(info.row.original?.id)}
            >
              <IconEdit width={18} />
            </Button>
          </Box>
        ),
      }),
    ],
    [t, onEditClick],
  );

  const table = useReactTable({
    data,
    columns,
    state: {
      columnFilters,
      sorting,
    },
    onColumnFiltersChange: setColumnFilters,
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
  });

  const handleDownload = () => {
    const dataToExport = data.map((dept) => ({
      [t('Field.Name')]: dept.name,
      [t('Field.Note')]: dept.note,
      [t('Field.Status')]: dept.isActivated ? t('Status.Active') : t('Status.Inactive'),
    }));

    const worksheet = XLSX.utils.json_to_sheet(dataToExport);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Departments');
    XLSX.writeFile(workbook, 'department-list.xlsx');
  };

  return (
    <DownloadCard title={t('Page.DepartmentList') || 'Department List'} onDownload={handleDownload}>
      <Grid container spacing={3}>
        <Grid size={12}>
          <Box>
            <TableContainer>
              <Table sx={{ whiteSpace: 'nowrap' }}>
                <TableHead>
                  {table.getHeaderGroups().map((headerGroup) => (
                    <TableRow key={headerGroup.id}>
                      {headerGroup.headers.map((header) => (
                        <TableCell key={header.id}>
                          <Typography
                            variant="h6"
                            mb={1}
                            className={
                              header.column.getCanSort() ? 'cursor-pointer select-none' : ''
                            }
                            onClick={header.column.getToggleSortingHandler()}
                          >
                            {header.isPlaceholder
                              ? null
                              : flexRender(header.column.columnDef.header, header.getContext())}
                            {(() => {
                              const sortState = header.column.getIsSorted();
                              if (sortState === 'asc') return ' 🔼';
                              if (sortState === 'desc') return ' 🔽';
                              return null;
                            })()}
                          </Typography>
                          {header.column.getCanFilter() && <Filter t={t} column={header.column} />}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))}
                </TableHead>
                <TableBody>
                  {loading ? (
                    <TableRow>
                      <TableCell colSpan={columns.length}>
                        <Box
                          sx={{
                            display: 'flex',
                            justifyContent: 'center',
                            alignItems: 'center',
                            height: 240,
                          }}
                        >
                          <CircularProgress />
                        </Box>
                      </TableCell>
                    </TableRow>
                  ) : table.getRowModel().rows.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={columns.length}>
                        <Box
                          sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                            justifyContent: 'center',
                            height: 240,
                          }}
                        >
                          <img
                            src={EmptyImage}
                            alt="No data"
                            style={{ width: 180, marginBottom: 12 }}
                          />
                          <Typography variant="body1" color="text.secondary">
                            {t('Message.NoData') || 'No data available'}
                          </Typography>
                        </Box>
                      </TableCell>
                    </TableRow>
                  ) : (
                    table.getRowModel().rows.map((row) => (
                      <TableRow key={row.id}>
                        {row.getVisibleCells().map((cell) => (
                          <TableCell key={cell.id}>
                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                          </TableCell>
                        ))}
                      </TableRow>
                    ))
                  )}
                </TableBody>
              </Table>
            </TableContainer>
            <Divider />

            <Stack gap={1} p={3} alignItems="center" direction="row" justifyContent="space-between">
              <Box display="flex" alignItems="center" gap={1}>
                <Button variant="contained" color="primary" onClick={() => table.reset()}>
                  {t('Action.Reset') || 'Reset'}
                </Button>
                <Typography variant="body1">
                  {table.getPrePaginationRowModel().rows.length} {t('Field.Rows') || 'Rows'}
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" gap={1}>
                <Stack direction="row" alignItems="center" gap={1}>
                  <Typography variant="body1">{t('Field.Page') || 'Page'}</Typography>
                  <Typography variant="body1" fontWeight={600}>
                    {table.getState().pagination.pageIndex + 1} {t('Field.Of') || 'of'}{' '}
                    {table.getPageCount()}
                  </Typography>
                </Stack>
                <Stack direction="row" alignItems="center" gap={1}>
                  | {t('Action.GoToPage') || 'Go to page'}:
                  <CustomTextField
                    type="number"
                    min="1"
                    max={table.getPageCount()}
                    defaultValue={table.getState().pagination.pageIndex + 1}
                    onChange={(e) => {
                      const page = e.target.value ? Number(e.target.value) - 1 : 0;
                      table.setPageIndex(page);
                    }}
                  />
                </Stack>
                <CustomSelect
                  value={table.getState().pagination.pageSize}
                  onChange={(e) => {
                    table.setPageSize(Number(e.target.value));
                  }}
                >
                  {[10, 15, 20, 25].map((pageSize) => (
                    <MenuItem key={pageSize} value={pageSize}>
                      {pageSize}
                    </MenuItem>
                  ))}
                </CustomSelect>

                <IconButton
                  size="small"
                  onClick={() => table.setPageIndex(0)}
                  disabled={!table.getCanPreviousPage()}
                >
                  <IconChevronsLeft />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => table.previousPage()}
                  disabled={!table.getCanPreviousPage()}
                >
                  <IconChevronLeft />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => table.nextPage()}
                  disabled={!table.getCanNextPage()}
                >
                  <IconChevronRight />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => table.setPageIndex(table.getPageCount() - 1)}
                  disabled={!table.getCanNextPage()}
                >
                  <IconChevronsRight />
                </IconButton>
              </Box>
            </Stack>
          </Box>
        </Grid>
      </Grid>
    </DownloadCard>
  );
};

export default DepartmentListTable;
