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
  Tooltip,
} from '@mui/material';
import CancelPresentationIcon from '@mui/icons-material/CancelPresentation';
import AssignmentTurnedInIcon from '@mui/icons-material/AssignmentTurnedIn';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';
import LaunchIcon from '@mui/icons-material/Launch';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';

import { format, isValid, parseISO } from 'date-fns';
import { StockTakeContext } from 'src/context/StockTakeContext';
import { useMasterData } from 'src/hooks/useMasterData';
import Logo from 'src/layouts/full/shared/logo/Logo';
import RequiredRole from 'src/components/guard';

const StockTakeDetail = () => {
  const { t } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();

  const { getStockTakeById, approveStockTake, cancelStockTake } = useContext(StockTakeContext);
  const { warehouses, requesters, products, units } = useMasterData();

  const [stockTake, setStockTake] = useState(null);
  const [loading, setLoading] = useState(true);

  const maps = useMemo(() => {
    const createMap = (arr) =>
      (arr || []).reduce((acc, item) => ({ ...acc, [item.id]: item.name }), {});
    return {
      warehouses: createMap(warehouses),
      requesters: createMap(requesters),
      products: createMap(products),
      units: createMap(units),
    };
  }, [warehouses, requesters, products, units]);

  useEffect(() => {
    const fetchDetail = async () => {
      setLoading(true);
      try {
        const data = await getStockTakeById(Number(id));
        if (data) setStockTake(data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };
    fetchDetail();
  }, [id, getStockTakeById]);

  if (loading)
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
        <CircularProgress />
      </Box>
    );

  if (!stockTake) return <Typography p={3}>{t('Message.NoData')}</Typography>;

  const createdDate =
    stockTake.createdDate && isValid(parseISO(stockTake.createdDate))
      ? format(parseISO(stockTake.createdDate), 'dd/MM/yyyy HH:mm')
      : '';

  const renderHandlingAction = (diff) => {
    if (diff === 0) {
      return (
        <Stack direction="row" alignItems="center" gap={1} color="success.main">
          <CheckCircleOutlineIcon fontSize="small" />
          <Typography variant="body2">{t('Status.Balanced') || 'Khớp'}</Typography>
        </Stack>
      );
    }

    const isMissing = diff < 0;
    const targetPath = isMissing
      ? `/inventory/stock-in/add/adjustment?refId=${stockTake.id}`
      : `/inventory/stock-out/add/adjustment?refId=${stockTake.id}`;
    const label = isMissing ? t('Menu.StockIn') : t('Menu.StockOut');
    return (
      <Tooltip title={`${t('Action.Create')} ${label}`}>
        <Button
          size="small"
          variant="text"
          color={isMissing ? 'info' : 'warning'}
          startIcon={<LaunchIcon size={14} />}
          component={Link}
          to={targetPath}
          state={{
            productId: null,
            reason: `${t('Field.Handling')} ${stockTake.code}`,
          }}
        >
          {label}
        </Button>
      </Tooltip>
    );
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'PENDING':
        return 'warning';

      case 'APPROVED':
        return 'success';

      case 'RETURNED':
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
    <>
      {stockTake.status?.toUpperCase() === 'PENDING' && (
        <RequiredRole allowedRoles={['Warehouse Keeper']}>
          <Box display="flex" gap={1} p={2} bgcolor="primary.light" borderRadius={1} mb={3}>
            <Button
              variant="contained"
              color="success"
              startIcon={<AssignmentTurnedInIcon />}
              onClick={async () => {
                await approveStockTake(stockTake.id);
                setStockTake((prev) => ({ ...prev, status: 'APPROVED' }));
              }}
            >
              {t('Action.Approve')}
            </Button>
            <Button
              variant="outlined"
              color="error"
              startIcon={<CancelPresentationIcon />}
              onClick={async () => {
                await cancelStockTake(stockTake.id);
                setStockTake((prev) => ({ ...prev, status: 'CANCELLED' }));
              }}
            >
              {t('Action.Cancel')}
            </Button>
          </Box>
        </RequiredRole>
      )}

      <Stack direction="row" alignItems="center" justifyContent="space-between" mb={3}>
        <Box>
          <Typography variant="h4" fontWeight={700} mb={0.5}>
            {stockTake.code || `#${stockTake.id}`}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {createdDate}
          </Typography>
        </Box>
        <Logo />
        <Chip
          label={getStatusLabel(stockTake.status)}
          color={getStatusColor(stockTake.status)}
          sx={{ fontWeight: 'bold' }}
        />
      </Stack>

      <Divider />

      <Grid container spacing={3} mt={1} mb={4}>
        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ p: 2 }}>
            <Typography variant="caption" color="text.secondary">
              {t('Field.Warehouse')}
            </Typography>
            <Typography variant="subtitle1" fontWeight={600}>
              {maps.warehouses[stockTake.warehouseId] || 'Unknown'}
            </Typography>
          </Paper>
        </Grid>
        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ p: 2 }}>
            <Typography variant="caption" color="text.secondary">
              {t('Entity.Requester')}
            </Typography>
            <Typography variant="subtitle1" fontWeight={600}>
              {maps.requesters[stockTake.requesterId] || 'Unknown'}
            </Typography>
          </Paper>
        </Grid>
        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ p: 2 }}>
            <Typography variant="caption" color="text.secondary">
              {t('Field.Purpose')}
            </Typography>
            <Typography variant="subtitle1" fontWeight={600}>
              {stockTake.purpose || t('Message.NoData')}
            </Typography>
          </Paper>
        </Grid>
      </Grid>

      <Paper variant="outlined">
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: 'grey.50' }}>
                <TableCell width="30%">{t('Entity.Product')}</TableCell>
                <TableCell align="center">{t('Field.SystemQuantity') || 'Sổ sách'}</TableCell>
                <TableCell align="center">{t('Field.ActualQuantity') || 'Thực tế'}</TableCell>
                <TableCell align="center">{t('Field.Discrepancy') || 'Lệch'}</TableCell>
                <TableCell align="center">{t('Field.Handling') || 'Xử trí'}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(stockTake.items ?? []).map((item, index) => {
                const diff = (item.actualQty ?? 0) - (item.sysQty ?? 0);
                return (
                  <TableRow key={index} hover>
                    <TableCell>
                      <Typography variant="subtitle2" fontWeight={600}>
                        {maps.products[item.productId] || item.productName || `#${item.productId}`}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {maps.units[item.unitId] || ''}
                      </Typography>
                    </TableCell>
                    <TableCell align="center">{item.sysQty}</TableCell>
                    <TableCell align="center">{item.actualQty}</TableCell>
                    <TableCell align="center">
                      <Chip
                        label={diff > 0 ? `+${diff}` : diff}
                        size="small"
                        color={diff === 0 ? 'default' : diff > 0 ? 'success' : 'error'}
                        variant="outlined"
                        sx={{ fontWeight: 'bold' }}
                      />
                    </TableCell>
                    <TableCell align="center">{renderHandlingAction(diff)}</TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      <Box display="flex" justifyContent="flex-end" gap={2} mt={3}>
        {stockTake.status?.toUpperCase() === 'PENDING' && (
          <Button
            variant="contained"
            color="secondary"
            startIcon={<EditIcon />}
            component={Link}
            to={`/inventory/stock-take/edit/${stockTake.id}`}
          >
            {t('Action.Edit')}
          </Button>
        )}
        <Button
          variant="outlined"
          color="inherit"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/inventory/stock-take/list')}
        >
          {t('Action.Back')}
        </Button>
      </Box>
    </>
  );
};

export default StockTakeDetail;
