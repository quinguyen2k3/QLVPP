import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import {
  Dialog,
  AppBar,
  Toolbar,
  IconButton,
  Typography,
  Slide,
  Box,
  Container,
  Grid,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  CircularProgress,
  Stack,
  Divider,
  Alert,
} from '@mui/material';
import { IconX, IconPrinter } from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';
import { format } from 'date-fns';

// Import API
import { inventoryApi } from 'src/api/inventory/inventoryApi';

// --- HIỆU ỨNG TRƯỢT LÊN (SLIDE UP) ---
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

// --- HELPER FORMAT ---
const formatNumber = (num) => new Intl.NumberFormat('vi-VN').format(num);
const formatDate = (date) => (date ? format(new Date(date), 'dd/MM/yyyy') : '...');

const InventorySnapshotDetail = ({ open, onClose, id }) => {
  const { t } = useTranslation();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (open && id) {
      const fetchData = async () => {
        setLoading(true);
        setError(null);
        try {
          const res = await inventoryApi.getById(id);

          // --- SỬA TẠI ĐÂY (QUAN TRỌNG) ---
          // JSON trả về có dạng: { success: true, data: { ... } }
          // Nên ta ưu tiên lấy res.data. Nếu không có (do interceptor đã lọc) thì lấy res.
          const realData = res?.data || res;

          setData(realData);
        } catch (err) {
          console.error(err);
          setError(err.message || 'Lỗi tải dữ liệu');
        } finally {
          setLoading(false);
        }
      };
      fetchData();
    } else {
      if (!open) setData(null);
    }
  }, [open, id]);

  return (
    <Dialog fullScreen open={open} onClose={onClose} TransitionComponent={Transition}>
      <AppBar
        sx={{ position: 'relative', boxShadow: 0, borderBottom: '1px solid #e0e0e0' }}
        color="default"
      >
        <Toolbar>
          <IconButton edge="start" color="inherit" onClick={onClose} aria-label="close">
            <IconX />
          </IconButton>
          <Typography sx={{ ml: 2, flex: 1 }} variant="h6" component="div">
            {t('Page.ReportDetail')}
          </Typography>

          <IconButton color="primary" onClick={() => window.print()}>
            <IconPrinter />
          </IconButton>
        </Toolbar>
      </AppBar>

      <Box sx={{ backgroundColor: '#f5f7fa', minHeight: '100vh', p: 3 }}>
        <Container maxWidth="lg">
          {loading ? (
            <Box display="flex" justifyContent="center" mt={10}>
              <CircularProgress />
            </Box>
          ) : error ? (
            <Alert severity="error" sx={{ mt: 2 }}>
              {error}
            </Alert>
          ) : data ? (
            <Paper elevation={0} sx={{ p: 4, borderRadius: 2, border: '1px solid #e0e0e0' }}>
              <Box mb={4}>
                <Typography
                  variant="h3"
                  align="center"
                  gutterBottom
                  color="primary.main"
                  fontWeight="bold"
                >
                  {`${t('Entity.Report')} ${t('Entity.Warehouse')}` || 'BÁO CÁO KIỂM KÊ KHO'}
                </Typography>

                <Box sx={{ backgroundColor: '#f8f9fa', p: 3, borderRadius: 2, mt: 2 }}>
                  <Grid container spacing={2}>
                    <Grid item xs={12} md={6}>
                      <Stack direction="row" spacing={1}>
                        <Typography fontWeight="bold" color="textSecondary">
                          {t('Menu.Warehouse') || 'Kho hàng'}:
                        </Typography>
                        <Typography variant="body1" fontWeight="500">
                          {data.warehouseName || t('Label.UnknownWarehouse') || '---'}
                        </Typography>
                      </Stack>
                    </Grid>

                    <Grid item xs={12} md={6}>
                      <Stack
                        direction="row"
                        spacing={4}
                        justifyContent={{ xs: 'flex-start', md: 'flex-end' }}
                      >
                        <Box>
                          <Typography fontWeight="bold" color="textSecondary" component="span">
                            {t('Field.FromDate') || 'Từ ngày'}:{' '}
                          </Typography>
                          <Typography component="span" fontWeight="500">
                            {formatDate(data.fromDate)}
                          </Typography>
                        </Box>
                        <Box>
                          <Typography fontWeight="bold" color="textSecondary" component="span">
                            {t('Field.ToDate') || 'Đến ngày'}:{' '}
                          </Typography>
                          <Typography component="span" fontWeight="500">
                            {formatDate(data.toDate)}
                          </Typography>
                        </Box>
                      </Stack>
                    </Grid>
                  </Grid>
                </Box>
              </Box>

              <TableContainer component={Box} sx={{ border: '1px solid #eee', borderRadius: 1 }}>
                <Table>
                  <TableHead sx={{ backgroundColor: '#eef2f6' }}>
                    <TableRow>
                      <TableCell align="center" width="50px">
                        <b>{t('Field.Table.No') || 'STT'}</b>
                      </TableCell>
                      <TableCell>
                        <b>{t('Entity.Product') || 'Tên mặt hàng'}</b>
                      </TableCell>
                      <TableCell align="center">
                        <b>{t('Entity.Unit') || 'ĐVT'}</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>{t('Field.OpeningQty') || 'Tồn đầu'}</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>{t('Field.TotalIn') || 'Tổng nhập'}</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>{t('Field.TotalOut') || 'Tổng xuất'}</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>{t('Field.ClosingQty') || 'Tồn cuối'}</b>
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {data.items && data.items.length > 0 ? (
                      data.items.map((row, index) => (
                        <TableRow key={index} hover>
                          <TableCell align="center">{index + 1}</TableCell>
                          <TableCell>
                            <Box>
                              <Typography variant="body2" fontWeight="600">
                                {row.productName}
                              </Typography>
                              {row.productSku && (
                                <Typography variant="caption" color="textSecondary">
                                  {row.productSku}
                                </Typography>
                              )}
                            </Box>
                          </TableCell>
                          <TableCell align="center">{row.unitName || '-'}</TableCell>

                          <TableCell align="right">{formatNumber(row.openingQty)}</TableCell>

                          <TableCell
                            align="right"
                            sx={{
                              color: 'success.main',
                              fontWeight: row.totalIn > 0 ? 'bold' : 'normal',
                            }}
                          >
                            {row.totalIn > 0 ? `+${formatNumber(row.totalIn)}` : '-'}
                          </TableCell>

                          <TableCell
                            align="right"
                            sx={{
                              color: 'error.main',
                              fontWeight: row.totalOut > 0 ? 'bold' : 'normal',
                            }}
                          >
                            {row.totalOut > 0 ? `-${formatNumber(row.totalOut)}` : '-'}
                          </TableCell>

                          <TableCell
                            align="right"
                            sx={{ fontWeight: 'bold', backgroundColor: '#fafafa' }}
                          >
                            {formatNumber(row.closingQty)}
                          </TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={7} align="center" sx={{ py: 5 }}>
                          <Typography color="textSecondary">
                            {t('Message.NoData') || 'Không có dữ liệu'}
                          </Typography>
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          ) : null}
        </Container>
      </Box>
    </Dialog>
  );
};

InventorySnapshotDetail.propTypes = {
  open: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
};

export default InventorySnapshotDetail;
