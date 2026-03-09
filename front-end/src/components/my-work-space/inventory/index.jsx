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
  Grid,
  Stack,
  CircularProgress,
  useTheme,
  Divider,
  IconButton,
  Paper,
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
} from '@tabler/icons';

import EmptyImage from 'src/assets/images/svgs/no-data.webp';
import { fetchInventoryByDepartment } from '../data/fetchInventoryDepartment';

const columnHelper = createColumnHelper();

function Filter({ column }) {
  const columnFilterValue = column.getFilterValue();
  const { filterVariant } = column.columnDef.meta || {};

  if (filterVariant === 'select') {
    return (
      <CustomSelect
        value={columnFilterValue ?? ''}
        onChange={(e) => column.setFilterValue(e.target.value)}
        fullWidth
        size="small"
        sx={{ minWidth: '130px' }}
      >
        <MenuItem value="">Tất cả</MenuItem>
        <MenuItem value="instock">Còn hàng</MenuItem>
        <MenuItem value="outofstock">Hết hàng</MenuItem>
      </CustomSelect>
    );
  }

  return (
    <CustomTextField
      placeholder="Tìm kiếm..."
      value={columnFilterValue || ''}
      onChange={(e) => column.setFilterValue(e.target.value)}
      fullWidth
      size="small"
    />
  );
}

const InventoryDepartmentList = () => {
  const theme = useTheme();
  const [inventoryData, setInventoryData] = React.useState([]);
  const [loading, setLoading] = React.useState(true);
  const [columnFilters, setColumnFilters] = React.useState([]);
  const [sorting, setSorting] = React.useState([]);

  React.useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      try {
        const response = await fetchInventoryByDepartment();
        const dataArray =
          response.data && Array.isArray(response.data)
            ? response.data
            : Array.isArray(response)
              ? response
              : [];
        setInventoryData(dataArray);
      } catch (error) {
        console.error('Error loading inventory:', error);
        setInventoryData([]);
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, []);

  const columns = React.useMemo(
    () => [
      columnHelper.accessor('productName', {
        header: 'Tên Sản Phẩm',
        size: 350,
        cell: (info) => (
          <Typography variant="subtitle1" fontWeight={600} color="textPrimary">
            {info.getValue()}
          </Typography>
        ),
      }),

      columnHelper.accessor('currentQuantity', {
        header: 'Số lượng',
        size: 150,
        cell: (info) => (
          <Typography variant="body1" fontWeight={500} textAlign="right" pr={2}>
            {info.getValue()?.toLocaleString()}
          </Typography>
        ),
      }),

      columnHelper.accessor('unitName', {
        header: 'ĐVT',
        size: 100,
        cell: (info) => (
          <Typography variant="body2" color="textSecondary">
            {info.getValue()}
          </Typography>
        ),
      }),

      columnHelper.accessor((row) => (row.currentQuantity > 0 ? 'instock' : 'outofstock'), {
        id: 'status',
        header: 'Trạng thái',
        size: 150,
        cell: (info) => {
          const status = info.getValue();
          return (
            <Chip
              label={status === 'instock' ? 'Còn hàng' : 'Hết hàng'}
              color={status === 'instock' ? 'success' : 'error'}
              variant="outlined"
              size="small"
              sx={{
                fontWeight: 600,
                border: 'none',
                bgcolor: status === 'instock' ? '#e8f5e9' : '#ffebee',
              }}
            />
          );
        },
        meta: { filterVariant: 'select' },
      }),
    ],
    [],
  );

  const table = useReactTable({
    data: inventoryData,
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
    const dataToExport = inventoryData.map((item) => ({
      'Tên Sản Phẩm': item.productName,
      'Số Lượng': item.currentQuantity,
      ĐVT: item.unitName,
      'Trạng Thái': item.currentQuantity > 0 ? 'Còn hàng' : 'Hết hàng',
    }));

    const worksheet = XLSX.utils.json_to_sheet(dataToExport);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Inventory');
    XLSX.writeFile(workbook, 'department-inventory.xlsx');
  };

  return (
    <DownloadCard title="Inventory List" onDownload={handleDownload}>
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
                          {header.column.getCanFilter() && <Filter column={header.column} />}
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
                            alt="No data"
                            style={{ width: 180, marginBottom: 12 }}
                          />
                          <Typography variant="body1" color="text.secondary">
                            No data available
                          </Typography>
                        </Box>
                      </TableCell>
                    </TableRow>
                  ) : (
                    table.getRowModel().rows.map((row) => (
                      <TableRow key={row.id} hover>
                        {row.getVisibleCells().map((cell) => (
                          <TableCell
                            key={cell.id}
                            align={cell.column.columnDef.meta?.align || 'left'}
                          >
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
                  {table.getPrePaginationRowModel().rows.length} Rows
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" gap={1}>
                <Stack direction="row" alignItems="center" gap={1}>
                  <Typography variant="body1">Page</Typography>
                  <Typography variant="body1" fontWeight={600}>
                    {table.getState().pagination.pageIndex + 1} of {table.getPageCount()}
                  </Typography>
                </Stack>
                <Stack direction="row" alignItems="center" gap={1}>
                  | Go to page:
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

export default InventoryDepartmentList;
