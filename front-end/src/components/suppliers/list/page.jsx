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
import { useTranslation } from 'react-i18next'; // Import i18n

const columnHelper = createColumnHelper();

/* ================= FILTER ================= */
function Filter({ column, t }) {
  const value = column.getFilterValue();
  const { filterVariant } = column.columnDef.meta || {};

  if (filterVariant === 'select') {
    return (
      <CustomSelect
        value={value ?? ''}
        onChange={(e) => column.setFilterValue(e.target.value)}
        size="small"
        fullWidth
      >
        <MenuItem value="">{t('Placeholder.AllStatus') || 'All'}</MenuItem>
        <MenuItem value="true">{t('Status.Active')}</MenuItem>
        <MenuItem value="false">{t('Status.Inactive')}</MenuItem>
      </CustomSelect>
    );
  }

  return (
    <CustomTextField
      size="small"
      fullWidth
      placeholder={t('Placeholder.Search')}
      value={value ?? ''}
      onChange={(e) => column.setFilterValue(e.target.value)}
    />
  );
}

/* ================= MAIN TABLE ================= */
const SupplierListTable = ({ suppliers = [], loading, onEditClick }) => {
  const { t } = useTranslation(); // Khởi tạo hook
  const [columnFilters, setColumnFilters] = React.useState([]);
  const [sorting, setSorting] = React.useState([]);

  /* ================= COLUMNS ================= */
  const columns = React.useMemo(
    () => [
      columnHelper.accessor((row) => row?.name ?? '', {
        id: 'name',
        header: t('Field.Name'), // Supplier Name -> Name
        size: 220,
      }),

      columnHelper.accessor((row) => row?.phone ?? '', {
        id: 'phone',
        header: t('Field.Phone'),
        size: 160,
      }),

      columnHelper.accessor((row) => row?.email ?? '', {
        id: 'email',
        header: t('Field.Email'),
        size: 260,
      }),

      columnHelper.accessor((row) => row?.isActivated ?? false, {
        header: t('Field.Status'),
        size: 140,
        cell: (info) => (
          <Chip
            label={info.getValue() ? t('Status.Active') : t('Status.Inactive')}
            color={info.getValue() ? 'success' : 'error'}
            size="small"
          />
        ),
        meta: { filterVariant: 'select' },
        filterFn: (row, columnId, filterValue) => {
          if (filterValue === '' || filterValue === undefined || filterValue === null) {
            return true;
          }
          return String(row.getValue(columnId)) === String(filterValue);
        },
      }),

      columnHelper.display({
        id: 'actions',
        header: '',
        size: 80,
        cell: (info) => (
          <Box display="flex" justifyContent="center">
            <Button
              size="small"
              sx={{ minWidth: 0, p: 1 }}
              onClick={() => onEditClick?.(info.row.original?.id)}
            >
              <IconEdit size={18} />
            </Button>
          </Box>
        ),
      }),
    ],
    [t, onEditClick],
  );

  const table = useReactTable({
    data: suppliers,
    columns,
    state: { columnFilters, sorting },
    onColumnFiltersChange: setColumnFilters,
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
  });

  const handleDownload = () => {
    const data = suppliers.map((s) => ({
      [t('Field.Name')]: s.name,
      [t('Field.Phone')]: s.phone,
      [t('Field.Email')]: s.email,
      [t('Field.Status')]: s.isActivated ? t('Status.Active') : t('Status.Inactive'),
    }));

    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Suppliers');
    XLSX.writeFile(wb, 'supplier-list.xlsx');
  };

  return (
    <DownloadCard title={t('Page.SupplierList')} onDownload={handleDownload}>
      <Grid container spacing={3}>
        <Grid item size={12}>
          <Box>
            <TableContainer>
              <Table
                sx={{
                  whiteSpace: 'nowrap',
                }}
              >
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

                          {header.column.getCanFilter() && <Filter column={header.column} t={t} />}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))}
                </TableHead>
                <TableBody>
                  {table.getRowModel().rows.length === 0 ? (
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
                            alt={t('Message.NoData')}
                            style={{ width: 180, marginBottom: 12 }}
                          />
                          <Typography variant="body1" color="text.secondary">
                            {t('Message.NoData')}
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

export default SupplierListTable;
