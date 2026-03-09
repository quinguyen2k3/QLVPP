import React, { useState, useEffect, useMemo } from 'react';
import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TablePagination,
  TextField,
  Tooltip,
  IconButton,
  Box,
  Typography,
  Stack,
  InputAdornment,
  Chip,
  CircularProgress,
  Snackbar,
  Alert,
  MenuItem,
} from '@mui/material';
import { IconEye, IconSearch, IconPackageImport, IconCheck } from '@tabler/icons-react';
import { Link } from 'react-router-dom';
import { useMasterData } from 'src/hooks/useMasterData';
import { stockOutApi } from 'src/api/inventory-transaction/stock-out/stockOutApi';
import EmptyImage from 'src/assets/images/svgs/no-data.webp';
import { useTranslation } from 'react-i18next';

const ReceiveTransferList = () => {
  const { t } = useTranslation();
  const { warehouses } = useMasterData();

  const [transferOuts, setTransferOuts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterStatus, setFilterStatus] = useState('incoming');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [alert, setAlert] = useState({ open: false, message: '', severity: 'success' });

  const warehouseLookup = useMemo(() => {
    return (warehouses || []).reduce((acc, cur) => {
      acc[cur.id] = cur.name;
      return acc;
    }, {});
  }, [warehouses]);

  const loadTransferRequests = async (statusMode) => {
    try {
      setLoading(true);
      let response;

      if (statusMode === 'incoming') {
        response = await stockOutApi.getTransferIncoming();
      } else {
        response = await stockOutApi.getTransferReceived();
      }

      setTransferOuts(response.data || response);
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadTransferRequests(filterStatus);
  }, [filterStatus]);

  const handleQuickReceive = async (id) => {
    try {
      setActionLoading(id);
      await stockOutApi.receive(id);
      setAlert({ open: true, message: t('Message.Success'), severity: 'success' });
      loadTransferRequests(filterStatus);
    } catch (error) {
      setAlert({ open: true, message: t('Message.Error'), severity: 'error' });
    } finally {
      setActionLoading(null);
    }
  };

  const filteredData = useMemo(() => {
    return transferOuts.filter((item) => {
      const keyword = searchTerm.toLowerCase();
      const code = item.code?.toLowerCase() || '';
      const fromWarehouse = (warehouseLookup[item.warehouseId] || '').toLowerCase();
      
      return code.includes(keyword) || fromWarehouse.includes(keyword);
    });
  }, [transferOuts, searchTerm, warehouseLookup]);

  const paginatedData = filteredData.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

  const getStatusColor = (status) => {
    switch (status?.toUpperCase()) {
      case 'PENDING':
        return 'warning';
      case 'APPROVED':
        return 'success';
      case 'COMPLETED':
        return 'success';
      case 'CANCELLED':
        return 'error';
      case 'REJECTED':
        return 'error';
      default:
        return 'default';
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return '';
    const key = status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();
    return t(`Status.${key}`);
  };

  return (
    <Box>
      <Typography variant="h5" mb={3} fontWeight={600}>
        {t('Menu.ReceiveTransfer')}
      </Typography>

      <Stack direction="row" spacing={2} mb={3}>
        <TextField
          size="small"
          placeholder={t('Placeholder.Search')}
          value={searchTerm}
          onChange={(e) => {
            setSearchTerm(e.target.value);
            setPage(0);
          }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <IconSearch size={18} />
              </InputAdornment>
            ),
          }}
          sx={{ width: '400px' }}
        />
        
        <TextField
          select
          size="small"
          value={filterStatus}
          onChange={(e) => {
            setFilterStatus(e.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="incoming">{t('Filter.Incoming', 'Đang chờ nhận')}</MenuItem>
          <MenuItem value="received">{t('Filter.Received', 'Đã nhận')}</MenuItem>
        </TextField>
      </Stack>

      <Box sx={{ bgcolor: 'background.paper', borderRadius: 1 }}>
        {loading ? (
          <Box display="flex" justifyContent="center" alignItems="center" height="300px">
            <CircularProgress />
          </Box>
        ) : (
          <>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>{t('Field.Code')}</TableCell>
                  <TableCell>{t('Field.FromWarehouse')}</TableCell>
                  <TableCell>{t('Field.Date')}</TableCell>
                  <TableCell>{t('Field.Status')}</TableCell>
                  <TableCell align="center">{t('Field.Action')}</TableCell>
                </TableRow>
              </TableHead>

              <TableBody>
                {filteredData.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={5} align="center">
                      <Box p={4}>
                        <img src={EmptyImage} width={120} alt="no-data" />
                        <Typography variant="body2" color="textSecondary" mt={2}>
                          {t('Message.NoTransferRequest')}
                        </Typography>
                      </Box>
                    </TableCell>
                  </TableRow>
                ) : (
                  paginatedData.map((item) => (
                    <TableRow key={item.id} hover>
                      <TableCell sx={{ fontWeight: 600 }}>{item.code}</TableCell>
                      <TableCell>{warehouseLookup[item.warehouseId] || 'N/A'}</TableCell>
                      <TableCell>{item.stockOutDate}</TableCell>
                      <TableCell>
                        <Chip
                          label={getStatusLabel(item.status)}
                          size="small"
                          color={getStatusColor(item.status)}
                          variant="filled"
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Stack direction="row" spacing={1} justifyContent="center">
                          <Tooltip title={t('Action.ViewOriginalExport')}>
                            <IconButton
                              color="primary"
                              component={Link}
                              to={`/inventory/stock-out/detail/${item.id}`}
                              size="small"
                            >
                              <IconEye size={22} />
                            </IconButton>
                          </Tooltip>

                          <Tooltip title={t('Action.QuickReceive')}>
                            <IconButton
                              color="success"
                              onClick={() => handleQuickReceive(item.id)}
                              disabled={actionLoading !== null || item.status?.toUpperCase() === 'COMPLETED'}
                              size="small"
                            >
                              {actionLoading === item.id ? (
                                <CircularProgress size={20} color="inherit" />
                              ) : (
                                <IconCheck size={22} />
                              )}
                            </IconButton>
                          </Tooltip>

                          <Tooltip title={t('Action.CreateStockIn')}>
                            <IconButton
                              color="secondary"
                              component={Link}
                              to={`/inventory/stock-in/add/transfer?refId=${item.id}`}
                              size="small"
                            >
                              <IconPackageImport size={22} />
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
              rowsPerPageOptions={[10, 25]}
              component="div"
              count={filteredData.length}
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
      </Box>

      <Snackbar
        open={alert.open}
        autoHideDuration={3000}
        onClose={() => setAlert({ ...alert, open: false })}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert severity={alert.severity} onClose={() => setAlert({ ...alert, open: false })}>
          {alert.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default ReceiveTransferList;