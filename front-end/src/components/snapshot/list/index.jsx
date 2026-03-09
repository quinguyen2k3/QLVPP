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
// import { Link } from 'react-router-dom'; // <-- Không cần dùng Link nữa
import {
  IconChevronLeft,
  IconChevronRight,
  IconChevronsLeft,
  IconChevronsRight,
  IconEye,
  IconLock,
} from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';
import { format } from 'date-fns';
import EmptyImage from 'src/assets/images/svgs/no-data.webp';
import { inventoryApi } from 'src/api/inventory/inventoryApi';

function Filter({ column, t }) {
  const columnFilterValue = column.getFilterValue();
  return (
    <CustomTextField
      placeholder={t('Placeholder.Search')}
      value={columnFilterValue || ''}
      onChange={(e) => column.setFilterValue(e.target.value)}
      fullWidth
    />
  );
}

const columnHelper = createColumnHelper();

// 1. Nhận prop onViewDetail từ cha
const InventorySnapshotList = ({ onViewDetail }) => {
  const { t } = useTranslation();
  const [snapshots, setSnapshots] = React.useState([]);
  const [columnFilters, setColumnFilters] = React.useState([]);
  // eslint-disable-next-line no-unused-vars
  const [isLoading, setIsLoading] = React.useState(true);

  const fetchSnapshots = React.useCallback(async () => {
    setIsLoading(true);
    try {
      const response = await inventoryApi.getMyWarehouseSnapshots();
      setSnapshots(response.data || response);
    } catch (error) {
      console.error('Error fetching snapshots:', error);
    } finally {
      setIsLoading(false);
    }
  }, []);

  React.useEffect(() => {
    fetchSnapshots();
  }, [fetchSnapshots]);

  const columns = React.useMemo(
    () => [
      columnHelper.accessor('fromDate', {
        header: t('Field.FromDate'),
        cell: (info) => format(new Date(info.getValue()), 'dd/MM/yyyy'),
      }),
      columnHelper.accessor('toDate', {
        header: t('Field.ToDate'),
        cell: (info) => format(new Date(info.getValue()), 'dd/MM/yyyy'),
      }),
      columnHelper.accessor('status', {
        header: t('Field.Status'),
        cell: (info) => {
          const isComplete = info.getValue() === 'COMPLETE' || info.getValue() === 'COMPLETED';
          return (
            <Chip
              label={isComplete ? t('Status.Completed') : t('Status.Pending')}
              color={isComplete ? 'success' : 'warning'}
              icon={isComplete ? <IconLock size={14} /> : undefined}
              size="small"
            />
          );
        },
      }),
      columnHelper.display({
        id: 'actions',
        header: '',
        cell: (info) => (
          // 2. Sửa nút View Detail tại đây
          <Button
            color="primary"
            variant="outlined"
            size="small"
            startIcon={<IconEye width={18} />}
            onClick={() => {
              // Gọi hàm callback được truyền từ cha
              if (onViewDetail) {
                onViewDetail(info.row.original.id);
              }
            }}
          >
            {t('Action.View')}
          </Button>
        ),
      }),
    ],
    [t, onViewDetail], // <-- Quan trọng: Thêm onViewDetail vào dependency array
  );

  const table = useReactTable({
    data: snapshots,
    columns,
    state: { columnFilters },
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    initialState: {
      pagination: { pageSize: 10 },
    },
  });

  const handleDownload = () => {
    if (snapshots.length === 0) {
      alert(t('Message.NoDataToExport'));
      return;
    }

    const dataToExport = snapshots.map((item) => ({
      [t('Field.FromDate')]: format(new Date(item.fromDate), 'dd/MM/yyyy'),
      [t('Field.ToDate')]: format(new Date(item.toDate), 'dd/MM/yyyy'),
      [t('Field.Status')]:
        item.status === 'COMPLETED' ? t('Status.Completed') : t('Status.Pending'),
    }));

    const worksheet = XLSX.utils.json_to_sheet(dataToExport);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Inventory Snapshots');

    const fileName = `Inventory_Closing_${format(new Date(), 'yyyyMMdd_HHmm')}.xlsx`;
    XLSX.writeFile(workbook, fileName);
  };

  return (
    <DownloadCard title={t('Page.ProductList')} onDownload={handleDownload}>
      <Grid container spacing={3}>
        <Grid item size={12}>
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

export default InventorySnapshotList;
