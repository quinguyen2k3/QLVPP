import React, { useContext, useEffect, useState, useMemo } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { vi, enUS } from 'date-fns/locale';
import { useTranslation } from 'react-i18next';
import {
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Box,
  Stack,
  Chip,
  Divider,
  Grid,
  CircularProgress,
} from '@mui/material';
import CancelPresentationIcon from '@mui/icons-material/CancelPresentation';
import AssignmentTurnedInIcon from '@mui/icons-material/AssignmentTurnedIn';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';

import { format, isValid, parseISO } from 'date-fns';

import { StockInContext } from 'src/context/StockInContext';
import { useMasterData } from 'src/hooks/useMasterData';
import Logo from 'src/layouts/full/shared/logo/Logo';

const STOCK_IN_TYPES = {
  PURCHASE: 1,
  TRANSFER: 2,
  RETURN: 3,
  ADJUSTMENT: 4,
};

const StockInDetail = () => {
  const { t, i18n } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();

  const { getStockInById, approveStockIn, cancelStockIn } = useContext(StockInContext);
  const { suppliers, warehouses, requesters, products, units, departments } = useMasterData();

  const [stockIn, setStockIn] = useState(null);
  const [loading, setLoading] = useState(true);

  const maps = useMemo(() => {
    const createMap = (arr) =>
      (arr || []).reduce((acc, item) => ({ ...acc, [item.id]: item.name }), {});
    return {
      suppliers: createMap(suppliers),
      warehouses: createMap(warehouses),
      requesters: createMap(requesters),
      products: createMap(products),
      units: createMap(units),
      departments: createMap(departments),
    };
  }, [suppliers, warehouses, requesters, products, units, departments]);

  useEffect(() => {
    const fetchDetail = async () => {
      setLoading(true);
      try {
        const data = await getStockInById(Number(id));
        if (data) {
          setStockIn(data);
        }
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };
    fetchDetail();
  }, [id, getStockInById]);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!stockIn) {
    return <Typography p={3}>{t('Message.NoData')}</Typography>;
  }

  const isVietnamese = i18n.language === 'vi';
  const dateLocale = isVietnamese ? vi : enUS;
  const dateFormatStr = isVietnamese ? "EEEE, 'ngày' dd 'tháng' MM, yyyy" : 'EEEE, MMMM dd, yyyy';

  const stockInDate =
    stockIn.stockInDate && isValid(parseISO(stockIn.stockInDate))
      ? format(parseISO(stockIn.stockInDate), dateFormatStr, { locale: dateLocale })
      : '';

  const getStatusColor = (status) => {
    switch (status) {
      case 'PENDING':
        return 'warning';
      case 'APPROVED':
        return 'success';
      case 'CANCELLED':
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

  const getTypeLabel = (type) => {
    switch (type) {
      case STOCK_IN_TYPES.PURCHASE:
        return t('StockInType.Purchase');
      case STOCK_IN_TYPES.TRANSFER:
        return t('StockInType.Transfer');
      case STOCK_IN_TYPES.RETURN:
        return t('StockInType.Return');
      case STOCK_IN_TYPES.ADJUSTMENT:
        return t('StockInType.Adjustment');
      default:
        return 'Unknown Type';
    }
  };

  const grandTotal = (stockIn.items ?? []).reduce((sum, item) => {
    const unitPrice = item.unitPrice ?? 0;
    const quantity = item.quantity ?? 0;
    return sum + unitPrice * quantity;
  }, 0);

  const renderSourceInfoCard = () => {
    let title = '';
    let content = '';

    switch (stockIn.type) {
      case STOCK_IN_TYPES.PURCHASE:
        title = t('Entity.Supplier');
        content = maps.suppliers[stockIn.supplierId] || 'Unknown Supplier';
        break;
      case STOCK_IN_TYPES.TRANSFER:
        title = t('Field.FromWarehouse');
        content = maps.warehouses[stockIn.fromWarehouseId] || 'Unknown Source Warehouse';
        break;
      case STOCK_IN_TYPES.RETURN:
        title = t('Field.FromDepartment');
        content = maps.departments[stockIn.fromDepartmentId] || 'Unknown Department';
        break;
      case STOCK_IN_TYPES.ADJUSTMENT:
        title = t('Field.Note');
        content = stockIn.note || t('Message.NoNote');
        break;
      default:
        return null;
    }

    return (
      <Paper variant="outlined" sx={{ height: '100%' }}>
        <Box p={3}>
          <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
            {title}
          </Typography>
          <Typography variant="subtitle1" fontWeight={600}>
            {content}
          </Typography>
        </Box>
      </Paper>
    );
  };

  const isPurchase = stockIn.type === STOCK_IN_TYPES.PURCHASE;

  return (
    <>
      {stockIn.status === 'PENDING' && (
        <Box
          display="flex"
          justifyContent="flex-start"
          gap={1}
          p={2}
          bgcolor="primary.light"
          borderRadius={1}
          mb={3}
        >
          <Button
            variant="contained"
            color="success"
            startIcon={<AssignmentTurnedInIcon />}
            onClick={async () => {
              try {
                await approveStockIn(stockIn.id);
                setStockIn((prev) => ({ ...prev, status: 'APPROVED' }));
              } catch (error) {
                console.error(error);
              }
            }}
          >
            {t('Action.Approve')}
          </Button>
          <Button
            variant="outlined"
            color="error"
            startIcon={<CancelPresentationIcon />}
            onClick={async () => {
              try {
                await cancelStockIn(stockIn.id);
                setStockIn((prev) => ({ ...prev, status: 'CANCELLED' }));
              } catch (error) {
                console.error(error);
              }
            }}
          >
            {t('Action.Cancel')}
          </Button>
        </Box>
      )}
      <Stack
        direction={{ xs: 'column', sm: 'row' }}
        alignItems="center"
        justifyContent="space-between"
        mb={3}
      >
        <Box textAlign={{ xs: 'center', sm: 'left' }}>
          <Stack direction="row" alignItems="center" gap={1} mb={0.5}>
            <Typography variant="h4" fontWeight={700}>
              {stockIn.code || `#${stockIn.id}`}
            </Typography>
            <Chip
              label={getTypeLabel(stockIn.type)}
              color="primary"
              size="small"
              variant="outlined"
            />
          </Stack>
          <Typography variant="body2" color="text.secondary">
            {stockInDate}
          </Typography>
        </Box>

        <Box my={{ xs: 2, sm: 0 }}>
          <Logo />
        </Box>

        <Box textAlign="right">
          <Chip
            label={getStatusLabel(stockIn.status)}
            color={getStatusColor(stockIn.status)}
            sx={{ fontWeight: 'bold', px: 1 }}
          />
        </Box>
      </Stack>

      <Divider />

      <Grid container spacing={3} mt={1} mb={4}>
        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ height: '100%' }}>
            <Box p={3}>
              <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
                {t('Field.ToWarehouse')}
              </Typography>
              <Typography variant="subtitle1" fontWeight={600}>
                {maps.warehouses[stockIn.warehouseId] || 'Unknown Warehouse'}
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ height: '100%' }}>
            <Box p={3}>
              <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
                {t('Entity.Requester')}
              </Typography>
              <Typography variant="subtitle1" fontWeight={600}>
                {maps.requesters[stockIn.requesterId] || 'Unknown Requester'}
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item size={{ xs: 12, sm: 4 }}>
          {renderSourceInfoCard()}
        </Grid>
      </Grid>

      <Paper variant="outlined">
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: 'grey.100' }}>
                <TableCell width="35%">
                  <Typography variant="subtitle2" fontWeight={600}>
                    {t('Entity.Product')}
                  </Typography>
                </TableCell>
                <TableCell width="15%">
                  <Typography variant="subtitle2" fontWeight={600}>
                    {t('Menu.Unit')}
                  </Typography>
                </TableCell>

                {isPurchase && (
                  <TableCell width="15%">
                    <Typography variant="subtitle2" fontWeight={600}>
                      {t('Field.Price')}
                    </Typography>
                  </TableCell>
                )}

                <TableCell width="15%">
                  <Typography variant="subtitle2" fontWeight={600}>
                    {t('Field.Quantity')}
                  </Typography>
                </TableCell>

                {isPurchase && (
                  <TableCell width="20%" align="right">
                    <Typography variant="subtitle2" fontWeight={600}>
                      {t('Field.Total')}
                    </Typography>
                  </TableCell>
                )}
              </TableRow>
            </TableHead>

            <TableBody>
              {(stockIn.items ?? []).map((item, index) => {
                const unitPrice = item.unitPrice ?? 0;
                const quantity = item.quantity ?? 0;
                const total = unitPrice * quantity;

                return (
                  <TableRow key={index} hover>
                    <TableCell>
                      <Box>
                        <Typography variant="subtitle2" fontWeight={600}>
                          {maps.products[item.productId] ||
                            item.productName ||
                            `Product #${item.productId}`}
                        </Typography>
                      </Box>
                    </TableCell>
                    <TableCell>{maps.units[item.unitId] || item.unitName || '-'}</TableCell>

                    {isPurchase && <TableCell>{unitPrice.toLocaleString()}</TableCell>}

                    <TableCell>
                      <Chip label={quantity} size="small" variant="outlined" />
                    </TableCell>

                    {isPurchase && (
                      <TableCell align="right">
                        <Typography fontWeight={600}>{total.toLocaleString()}</Typography>
                      </TableCell>
                    )}
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {stockIn.note && (
        <Paper variant="outlined" sx={{ mt: 3, bgcolor: 'grey.50' }}>
          <Box p={3}>
            <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
              {t('Field.Note')}
            </Typography>
            <Typography variant="body1">{stockIn.note}</Typography>
          </Box>
        </Paper>
      )}

      {isPurchase && (
        <Box p={3} bgcolor="primary.light" mt={3} borderRadius={1}>
          <Box display="flex" justifyContent="flex-end" gap={3}>
            <Typography variant="h6" fontWeight={600}>
              {t('Field.GrandTotal')}:
            </Typography>
            <Typography variant="h6" fontWeight={700} color="primary.main">
              {grandTotal.toLocaleString()}
            </Typography>
          </Box>
        </Box>
      )}

      <Box display="flex" justifyContent="flex-end" gap={2} mt={3}>
        {stockIn.status === 'PENDING' && (
          <Button
            variant="contained"
            color="secondary"
            startIcon={<EditIcon />}
            component={Link}
            to={`/inventory/stock-in/edit/${stockIn.id}`}
          >
            {t('Action.Edit')}
          </Button>
        )}

        <Button
          variant="outlined"
          color="inherit"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/inventory/stock-in/list')}
        >
          {t('Action.Back')}
        </Button>
      </Box>
    </>
  );
};

export default StockInDetail;
