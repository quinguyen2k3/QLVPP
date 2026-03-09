import React, { useContext, useEffect, useState, useMemo } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
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
import { vi, enUS } from 'date-fns/locale';

import { StockOutContext } from 'src/context/StockOutContext';
import { useMasterData } from 'src/hooks/useMasterData';
import Logo from 'src/layouts/full/shared/logo/Logo';

const STOCK_OUT_TYPES = {
  USAGE: 1,
  TRANSFER: 2,
  ADJUSTMENT: 3,
};

const StockOutDetail = () => {
  const { t, i18n } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();

  const { getStockOutById, approveStockOut, cancelStockOut } = useContext(StockOutContext);
  const { warehouses, departments, requesters, products, units } = useMasterData();

  const [stockOut, setStockOut] = useState(null);
  const [loading, setLoading] = useState(true);

  const maps = useMemo(() => {
    const createMap = (arr) =>
      (arr || []).reduce((acc, item) => ({ ...acc, [item.id]: item.name }), {});
    return {
      warehouses: createMap(warehouses),
      departments: createMap(departments),
      requesters: createMap(requesters),
      products: createMap(products),
      units: createMap(units),
    };
  }, [warehouses, departments, requesters, products, units]);

  useEffect(() => {
    const fetchDetail = async () => {
      setLoading(true);
      try {
        const data = await getStockOutById(Number(id));
        if (data) setStockOut(data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };
    fetchDetail();
  }, [id, getStockOutById]);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!stockOut) {
    return <Typography p={3}>{t('Message.NoData')}</Typography>;
  }

  const isVietnamese = i18n.language === 'vi';
  const dateLocale = isVietnamese ? vi : enUS;
  const dateFormatStr = isVietnamese ? "EEEE, 'ngày' dd 'tháng' MM, yyyy" : 'EEEE, MMMM dd, yyyy';

  const stockOutDate =
    stockOut.stockOutDate && isValid(parseISO(stockOut.stockOutDate))
      ? format(parseISO(stockOut.stockOutDate), dateFormatStr, { locale: dateLocale })
      : '';

  const getStatusColor = (status) => {
    switch (status?.toUpperCase()) {
      case 'PENDING': return 'warning';
      case 'APPROVED': return 'success';
      case 'COMPLETED': return 'success';
      case 'CANCELLED': return 'error';
      case 'REJECTED': return 'error';
      default: return 'default';
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return '';
    const key = status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();
    return t(`Status.${key}`);
  };

  const getTypeLabel = (type) => {
    switch (type) {
      case STOCK_OUT_TYPES.USAGE: return t('StockOutType.Usage');
      case STOCK_OUT_TYPES.TRANSFER: return t('StockOutType.Transfer');
      case STOCK_OUT_TYPES.ADJUSTMENT: return t('StockOutType.Adjustment');
      default: return 'Unknown Type';
    }
  };

  const renderDestinationInfoCard = () => {
    let title = '';
    let content = '';

    switch (stockOut.type) {
      case STOCK_OUT_TYPES.USAGE:
        title = t('Entity.Department');
        content = maps.departments[stockOut.departmentId] || 'Unknown Department';
        break;
      case STOCK_OUT_TYPES.TRANSFER:
        title = t('Field.ToWarehouse');
        content = maps.warehouses[stockOut.toWarehouseId] || 'Unknown Destination Warehouse';
        break;
      case STOCK_OUT_TYPES.ADJUSTMENT:
        title = t('Field.Note');
        content = stockOut.note || t('Message.NoNote');
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

  return (
    <>
      {stockOut.status === 'PENDING' && (
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
              const success = await approveStockOut(stockOut.id);
              if (success) setStockOut((prev) => ({ ...prev, status: 'APPROVED' }));
            }}
          >
            {t('Action.Approve')}
          </Button>
          <Button
            variant="outlined"
            color="error"
            startIcon={<CancelPresentationIcon />}
            onClick={async () => {
              const success = await cancelStockOut(stockOut.id);
              if (success) setStockOut((prev) => ({ ...prev, status: 'CANCELLED' }));
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
              {stockOut.code || `#${stockOut.id}`}
            </Typography>
            <Chip 
              label={getTypeLabel(stockOut.type)} 
              color="primary" 
              size="small" 
              variant="outlined" 
            />
          </Stack>
          <Typography variant="body2" color="text.secondary">
            {stockOutDate}
          </Typography>
        </Box>

        <Box my={{ xs: 2, sm: 0 }}>
          <Logo />
        </Box>

        <Box textAlign="right">
          <Chip
            label={getStatusLabel(stockOut.status)}
            color={getStatusColor(stockOut.status)}
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
                {t('Field.FromWarehouse')}
              </Typography>
              <Typography variant="subtitle1" fontWeight={600}>
                {maps.warehouses[stockOut.warehouseId] || 'Unknown Warehouse'}
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
                {maps.requesters[stockOut.requesterId] || 'Unknown Requester'}
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item size={{ xs: 12, sm: 4 }}>
          {renderDestinationInfoCard()}
        </Grid>
      </Grid>

      <Paper variant="outlined">
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: 'grey.100' }}>
                <TableCell width="50%">
                  <Typography variant="subtitle2" fontWeight={600}>{t('Entity.Product')}</Typography>
                </TableCell>
                <TableCell width="25%">
                  <Typography variant="subtitle2" fontWeight={600}>{t('Menu.Unit')}</Typography>
                </TableCell>
                <TableCell width="25%">
                  <Typography variant="subtitle2" fontWeight={600}>{t('Field.Quantity')}</Typography>
                </TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {(stockOut.items ?? []).map((item, index) => (
                <TableRow key={index} hover>
                  <TableCell>
                    <Typography variant="subtitle2" fontWeight={600}>
                      {maps.products[item.productId] || item.productName || `Product #${item.productId}`}
                    </Typography>
                  </TableCell>
                  <TableCell>{maps.units[item.unitId] || item.unitName || '-'}</TableCell>
                  <TableCell>
                    <Chip label={item.quantity} size="small" variant="outlined" />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {stockOut.note && stockOut.type !== STOCK_OUT_TYPES.ADJUSTMENT && (
        <Box mt={3} p={2} bgcolor="grey.50" borderRadius={1} border="1px solid" borderColor="grey.200">
           <Typography variant="subtitle2" color="text.secondary" mb={0.5}>{t('Field.Note')}:</Typography>
           <Typography variant="body2">{stockOut.note}</Typography>
        </Box>
      )}

      <Box display="flex" justifyContent="flex-end" gap={2} mt={3}>
        {stockOut.status === 'PENDING' && (
          <Button
            variant="contained"
            color="secondary"
            startIcon={<EditIcon />}
            component={Link}
            to={`/inventory/stock-out/edit/${stockOut.id}`}
          >
            {t('Action.Edit')}
          </Button>
        )}

        <Button
          variant="outlined"
          color="inherit"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/inventory/stock-out/list')}
        >
          {t('Action.Back')}
        </Button>
      </Box>
    </>
  );
};

export default StockOutDetail;