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
  Divider,
  IconButton,
  Tooltip,
  Grid,
  Stack,
} from '@mui/material';

import {
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
  createColumnHelper,
} from '@tanstack/react-table';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {
  IconChevronLeft,
  IconChevronRight,
  IconEye,
  IconClipboardCheck,
  IconChevronsLeft,
  IconChevronsRight
} from '@tabler/icons';

import DownloadCard from 'src/components/shared/DownloadCard';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import CustomSelect from 'src/components/forms/theme-elements/CustomSelect';
import EmptyImage from 'src/assets/images/svgs/no-data.webp';

import { stockOutApi } from 'src/api/inventory-transaction/stock-out/stockOutApi';
import { useMasterData } from 'src/hooks/useMasterData';

function Filter({ column, t }) {
  const columnFilterValue = column.getFilterValue();
  const { filterVariant, filterOptions } = column.columnDef.meta || {};

  if (filterVariant === 'select') {
    return (
      <CustomSelect
        value={columnFilterValue ?? ''}
        onChange={(e) => column.setFilterValue(e.target.value)}
        fullWidth
      >
        <MenuItem value="">{t('All')}</MenuItem>
        {filterOptions?.map((opt) => (
          <MenuItem key={opt.value} value={opt.value}>
            {t(opt.label)}
          </MenuItem>
        ))}
      </CustomSelect>
    );
  }

  return (
    <CustomTextField
      placeholder={`${t('Search')}...`}
      value={columnFilterValue || ''}
      onChange={(e) => column.setFilterValue(e.target.value)}
      fullWidth
    />
  );
}

const columnHelper = createColumnHelper();

const IncomingStockTable = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [columnFilters, setColumnFilters] = React.useState([]);
  
  const [data, setData] = React.useState([]);
  const [loading, setLoading] = React.useState(true);

  const { warehouses, loading: masterLoading } = useMasterData();

  const fetchIncomingData = async () => {
    setLoading(true);
    try {
      const response = await stockOutApi.getIncomingForDept();
      setData(response?.data || response || []);
    } catch (error) {
      console.error('Failed to fetch incoming stock', error);
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => {
    fetchIncomingData();
  }, []);

  const getWarehouseName = (id) => {
    if (masterLoading) return t('Loading');
    return warehouses.find((w) => w.id === id)?.name || `${t('UnknownWarehouse')} (#${id})`;
  };

  // Sửa logic hàm này: gọi API receive trực tiếp thay vì mở Dialog
  const handleConfirmReceive = async (id) => {
    try {
      await stockOutApi.receive(id);
      fetchIncomingData(); // Tải lại dữ liệu sau khi nhận thành công
    } catch (error) {
      console.error('Error receiving stock:', error);
    }
  };

  const columns = React.useMemo(
    () => [
      columnHelper.accessor('code', {
        header: t('Field.Code'),
        cell: (info) => (
          <Typography variant="subtitle1" fontWeight={600} color="primary">
            {info.getValue() || 'N/A'}
          </Typography>
        ),
      }),
      columnHelper.accessor('stockOutDate', {
        header: t('Field.Date'),
        cell: (info) => (
          <Typography variant="body1">
            {info.getValue() ? new Date(info.getValue()).toLocaleDateString('vi-VN') : 'N/A'}
          </Typography>
        ),
      }),
      columnHelper.accessor('warehouseId', {
        header: t('Field.Warehouse'),
        cell: (info) => (
          <Typography variant="body1">{getWarehouseName(info.getValue())}</Typography>
        ),
      }),
      columnHelper.accessor('status', {
        header: t('Field.Status'),
        cell: (info) => <Chip label={t(info.getValue())} color="info" size="small" />,
        meta: {
          filterVariant: 'select',
          filterOptions: [{ label: 'APPROVED', value: 'APPROVED' }],
        },
      }),
      columnHelper.display({
        id: 'actions',
        cell: (info) => (
          <Stack direction="row" spacing={1}>
            <Tooltip title={t('Action.View')}>
              <IconButton
                color="primary"
                size="small"
                onClick={() => navigate(`/inventory/stock-out/detail/${info.row.original.id}`)}
              >
                <IconEye size={20} />
              </IconButton>
            </Tooltip>
            <Tooltip title={t('Action.Receive')}>
              <IconButton
                color="success"
                size="small"
                // Sửa sự kiện onClick ở đây: truyền id vào hàm gọi API
                onClick={() => handleConfirmReceive(info.row.original.id)}
              >
                <IconClipboardCheck size={20} />
              </IconButton>
            </Tooltip>
          </Stack>
        ),
      }),
    ],
    [warehouses, masterLoading, t],
  );

  const table = useReactTable({
    data,
    columns,
    state: { columnFilters },
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
  });

  const handleDownload = () => {
    const dataToExport = data.map((item) => ({
      [t('Code')]: item.code,
      [t('Date')]: item.stockOutDate ? new Date(item.stockOutDate).toLocaleDateString('vi-VN') : '',
      [t('Warehouse')]: getWarehouseName(item.warehouseId),
      [t('Status')]: t(item.status),
    }));
    const worksheet = XLSX.utils.json_to_sheet(dataToExport);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Incoming Stock');
    XLSX.writeFile(workbook, 'incoming-stock.xlsx');
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

export default IncomingStockTable;