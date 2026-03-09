import React, { useContext, useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TablePagination,
  TextField,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Tooltip,
  IconButton,
  Box,
  Typography,
  Stack,
  InputAdornment,
  Chip,
  Menu,
  MenuItem,
  CircularProgress,
} from '@mui/material';
import { Grid } from '@mui/material';
import {
  IconEdit,
  IconEye,
  IconListDetails,
  IconSearch,
  IconTrash,
  IconCheck,
  IconX,
  IconClock,
  IconFilter,
  IconChevronDown,
  IconRotateClockwise,
  IconArchive,
} from '@tabler/icons-react';
import { Link } from 'react-router-dom';
import CustomCheckbox from 'src/components/forms/theme-elements/CustomCheckbox';
import { StockInContext } from '../../../../context/StockInContext';
import { useMasterData } from 'src/hooks/useMasterData';
import EmptyImage from 'src/assets/images/svgs/no-data.webp';
import { useTranslation } from 'react-i18next';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { format } from 'date-fns';

const StockInList = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const { stockIns, loading, deleteStockIn, fetchAllStockIns, fetchMyRequests, fetchToApprove } =
    useContext(StockInContext);

  const { requesters } = useMasterData();

  const [searchTerm, setSearchTerm] = useState('');
  const [activeTab, setActiveTab] = useState('All');
  const [selectedItems, setSelectedItems] = useState([]);
  const [selectAll, setSelectAll] = useState(false);

  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [deleteId, setDeleteId] = useState(null);

  const [viewMode, setViewMode] = useState('All');
  const [anchorEl, setAnchorEl] = useState(null);
  const openMenu = Boolean(anchorEl);

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);

  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);

  const isFiltering = searchTerm.length > 0 || startDate !== null || endDate !== null;

  const requesterLookup = useMemo(() => {
    return (requesters || []).reduce((acc, cur) => {
      acc[cur.id] = cur.name;
      return acc;
    }, {});
  }, [requesters]);

  useEffect(() => {
    const loadData = () => {
      switch (viewMode) {
        case 'MyRequest':
          fetchMyRequests();
          break;
        case 'ToApprove':
          fetchToApprove();
          break;
        case 'All':
        default:
          fetchAllStockIns();
          break;
      }
    };
    loadData();
  }, [viewMode]);

  const handleClickMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setAnchorEl(null);
  };

  const handleSelectViewMode = (mode) => {
    setViewMode(mode);
    setPage(0);
    handleCloseMenu();
  };

  const handleResetFilters = () => {
    setSearchTerm('');
    setStartDate(null);
    setEndDate(null);
  };

  const getTypeLabel = (type) => {
    switch (type) {
      case 1:
        return t('StockInType.Purchase') || 'Mua hàng';
      case 2:
        return t('StockInType.Transfer') || 'Điều chuyển';
      case 3:
        return t('StockInType.Return') || 'Trả hàng';
      case 4:
        return t('StockInType.Adjustment') || 'Kiểm kê';
      default:
        return t('StockInType.Unknown') || 'N/A';
    }
  };

  // --- LOGIC FILTER ---
  const filteredStockIns = stockIns.filter((item) => {
    const keyword = searchTerm.toLowerCase();
    const requesterName = (requesterLookup[item.requesterId] || '').toLowerCase();
    const code = item.code?.toLowerCase() || '';
    const matchSearch = requesterName.includes(keyword) || code.includes(keyword);

    const matchStatus = activeTab === 'All' || item.status === activeTab.toUpperCase();

    let matchDate = true;
    if (startDate || endDate) {
      if (!item.stockInDate) return false;

      const itemDate = new Date(item.stockInDate);
      itemDate.setHours(0, 0, 0, 0);

      if (startDate) {
        const start = new Date(startDate);
        start.setHours(0, 0, 0, 0);
        if (itemDate < start) matchDate = false;
      }

      if (endDate) {
        const end = new Date(endDate);
        end.setHours(0, 0, 0, 0);
        if (itemDate > end) matchDate = false;
      }
    }

    return matchSearch && matchStatus && matchDate;
  });

  const isEmpty = filteredStockIns.length === 0;

  const paginatedStockIns = filteredStockIns.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage,
  );

  const Pending = filteredStockIns.filter((s) => s.status === 'PENDING').length;
  const Approved = filteredStockIns.filter((s) => s.status === 'APPROVED').length;
  const Cancelled = filteredStockIns.filter((s) => s.status === 'CANCELLED').length;

  const toggleSelectAll = () => {
    const value = !selectAll;
    setSelectAll(value);
    setSelectedItems(value ? filteredStockIns.map((s) => s.id) : []);
  };

  const toggleSelectItem = (id) => {
    setSelectedItems((prev) => (prev.includes(id) ? prev.filter((i) => i !== id) : [...prev, id]));
  };

  const handleConfirmDelete = async () => {
    if (deleteId !== null) {
      await deleteStockIn(deleteId);
      if (selectedItems.includes(deleteId)) {
        setSelectedItems((prev) => prev.filter((id) => id !== deleteId));
      }
    } else {
      for (const id of selectedItems) {
        await deleteStockIn(id);
      }
      setSelectedItems([]);
      setSelectAll(false);
    }
    setDeleteId(null);
    setOpenDeleteDialog(false);
  };

  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setDeleteId(null);
  };

  const getViewLabel = () => {
    switch (viewMode) {
      case 'MyRequest':
        return t('Field.MyRequests');
      case 'ToApprove':
        return t('Field.PendingToApprove');
      default:
        return t('Field.AllRecords');
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return '';
    const key = status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();
    return t(`Status.${key}`);
  };

  const getStatusColor = (status) => {
    if (!status) return 'default';
    switch (status.toUpperCase()) {
      case 'APPROVED':
        return 'success';
      case 'PENDING':
        return 'warning';
      case 'CANCELLED':
        return 'error';
      default:
        return 'default';
    }
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Box>
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="primary.light"
              p={3}
              onClick={() => setActiveTab('All')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="primary.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconListDetails color="white" />
                </Box>
                <Box>
                  <Typography>{t('Field.AllRecords')}</Typography>
                  <Typography fontWeight={500}>{filteredStockIns.length}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>

          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="warning.light"
              p={3}
              onClick={() => setActiveTab('Pending')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="warning.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconClock color="white" />
                </Box>
                <Box>
                  <Typography>{t('Status.Pending')}</Typography>
                  <Typography fontWeight={500}>{Pending}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>

          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="success.light"
              p={3}
              onClick={() => setActiveTab('Approved')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="success.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconCheck color="white" />
                </Box>
                <Box>
                  <Typography>{t('Status.Approved')}</Typography>
                  <Typography fontWeight={500}>{Approved}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>

          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="error.light"
              p={3}
              onClick={() => setActiveTab('Cancelled')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="error.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconX color="white" />
                </Box>
                <Box>
                  <Typography>{t('Status.Cancelled')}</Typography>
                  <Typography fontWeight={500}>{Cancelled}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>
        </Grid>

        <Stack
          mt={3}
          direction={{ xs: 'column', md: 'row' }}
          spacing={2}
          justifyContent="space-between"
          alignItems="center"
        >
          <Box display="flex" gap={2} flexGrow={1} flexWrap="wrap" alignItems="center">
            <TextField
              size="small"
              placeholder={t('Placeholder.Search')}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconSearch size={16} />
                  </InputAdornment>
                ),
              }}
              sx={{ width: { xs: '100%', sm: '250px' } }}
            />

            <DatePicker
              label={t('Field.FromDate') || 'Từ ngày'}
              value={startDate}
              onChange={(newValue) => setStartDate(newValue)}
              slotProps={{
                textField: { size: 'small', sx: { width: { xs: '100%', sm: '150px' } } },
              }}
            />
            <DatePicker
              label={t('Field.ToDate') || 'Đến ngày'}
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
              slotProps={{
                textField: { size: 'small', sx: { width: { xs: '100%', sm: '150px' } } },
              }}
            />

            {isFiltering && (
              <Tooltip title={t('Action.ClearFilter') || 'Xóa bộ lọc'}>
                <IconButton onClick={handleResetFilters} color="secondary">
                  <IconRotateClockwise size={20} />
                </IconButton>
              </Tooltip>
            )}
          </Box>

          <Box display="flex" gap={1} alignItems="center">
            <Box>
              <Button
                variant="outlined"
                color="inherit"
                onClick={handleClickMenu}
                startIcon={<IconFilter size={18} />}
                endIcon={<IconChevronDown size={18} />}
                size="medium"
                sx={{ height: '36px', minWidth: '160px', justifyContent: 'space-between' }}
              >
                {getViewLabel()}
              </Button>
              <Menu
                anchorEl={anchorEl}
                open={openMenu}
                onClose={handleCloseMenu}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
                transformOrigin={{ vertical: 'top', horizontal: 'left' }}
              >
                <MenuItem onClick={() => handleSelectViewMode('All')} selected={viewMode === 'All'}>
                  {t('Field.AllRecords')}
                </MenuItem>
                <MenuItem
                  onClick={() => handleSelectViewMode('MyRequest')}
                  selected={viewMode === 'MyRequest'}
                >
                  {t('Field.MyRequests')}
                </MenuItem>
                <MenuItem
                  onClick={() => handleSelectViewMode('ToApprove')}
                  selected={viewMode === 'ToApprove'}
                >
                  {t('Field.PendingToApprove')}
                </MenuItem>
              </Menu>
            </Box>
            {selectAll && selectedItems.length > 0 && (
              <Button
                color="error"
                variant="outlined"
                onClick={() => {
                  setDeleteId(null);
                  setOpenDeleteDialog(true);
                }}
                startIcon={<IconTrash />}
                sx={{ height: '36px' }}
              >
                {t('Action.Delete')}
              </Button>
            )}

            <Button
              variant="contained"
              component={Link}
              to="/inventory/stock-in/add"
              sx={{ height: '36px' }}
            >
              {t('Action.Add')}
            </Button>
          </Box>
        </Stack>

        <Box mt={2} sx={{ overflowX: 'auto' }}>
          {loading ? (
            <Box display="flex" justifyContent="center" alignItems="center" height="200px">
              <CircularProgress />
            </Box>
          ) : (
            <>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell padding="checkbox">
                      <CustomCheckbox
                        checked={selectAll && !isEmpty}
                        disabled={isEmpty}
                        onChange={toggleSelectAll}
                      />
                    </TableCell>
                    <TableCell>{t('Field.Code')}</TableCell>
                    <TableCell>{t('Field.Type')}</TableCell>
                    <TableCell>{t('Entity.Requester')}</TableCell>
                    <TableCell>{t('Field.Date')}</TableCell>
                    <TableCell>{t('Field.Status')}</TableCell>
                    <TableCell align="center">{t('Field.Action')}</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {isEmpty ? (
                    <TableRow>
                      <TableCell colSpan={7}>
                        <Box
                          sx={{
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            height: '200px',
                          }}
                        >
                          <img
                            src={EmptyImage}
                            alt={t('Message.NoData')}
                            style={{ maxWidth: '100%', maxHeight: '100%' }}
                          />
                        </Box>
                      </TableCell>
                    </TableRow>
                  ) : (
                    paginatedStockIns.map((item) => (
                      <TableRow key={item.id}>
                        <TableCell padding="checkbox">
                          <CustomCheckbox
                            checked={selectedItems.includes(item.id)}
                            onChange={() => toggleSelectItem(item.id)}
                          />
                        </TableCell>
                        <TableCell>
                          <Typography variant="subtitle2" fontWeight={600}>
                            {item.code || `PNK-${item.id}`}
                          </Typography>
                        </TableCell>
                        <TableCell>
                          <Typography variant="body2">{getTypeLabel(item.type)}</Typography>
                        </TableCell>
                        <TableCell>
                          <Typography variant="body2">
                            {requesterLookup[item.requesterId] || 'N/A'}
                          </Typography>
                        </TableCell>

                        <TableCell>
                          {item.stockInDate ? format(new Date(item.stockInDate), 'dd/MM/yyyy') : ''}
                        </TableCell>

                        <TableCell>
                          <Chip
                            size="small"
                            label={getStatusLabel(item.status)}
                            color={getStatusColor(item.status)}
                          />
                        </TableCell>
                        <TableCell align="center">
                          <Stack direction="row" justifyContent="center" spacing={1}>
                            <Tooltip title={t('Action.View')}>
                              <IconButton
                                component={Link}
                                to={`/inventory/stock-in/detail/${item.id}`}
                                color="primary"
                              >
                                <IconEye />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title={t('Action.Edit')}>
                              <IconButton
                                component={Link}
                                to={`/inventory/stock-in/edit/${item.id}`}
                                color="success"
                              >
                                <IconEdit />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title={t('Action.Delete')}>
                              <IconButton
                                color="error"
                                onClick={() => {
                                  setDeleteId(item.id);
                                  setOpenDeleteDialog(true);
                                }}
                              >
                                <IconTrash />
                              </IconButton>
                            </Tooltip>
                          </Stack>
                        </TableCell>
                      </TableRow>
                    ))
                  )}
                </TableBody>
              </Table>
              <TablePagination
                rowsPerPageOptions={[5, 10, 25]}
                component="div"
                count={filteredStockIns.length}
                rowsPerPage={rowsPerPage}
                page={page}
                onPageChange={(e, p) => setPage(p)}
                onRowsPerPageChange={(e) => {
                  setRowsPerPage(parseInt(e.target.value, 10));
                  setPage(0);
                }}
              />
            </>
          )}
          <Button onClick={() => navigate('/inventory/transfer/list')} startIcon={<IconArchive />}>
            {t('Page.TransferList')}
          </Button>
        </Box>

        <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
          <DialogTitle>{t('Action.Delete')}</DialogTitle>
          <DialogContent>{t('Message.ConfirmDelete')}</DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDeleteDialog}>{t('Action.Cancel')}</Button>
            <Button color="error" variant="outlined" onClick={handleConfirmDelete}>
              {t('Action.Delete')}
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </LocalizationProvider>
  );
};

export default StockInList;
