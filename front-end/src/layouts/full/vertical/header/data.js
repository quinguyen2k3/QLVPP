import img1 from 'src/assets/images/profile/user-1.jpg';
import img2 from 'src/assets/images/profile/user-2.jpg';
import img3 from 'src/assets/images/profile/user-3.jpg';
import img4 from 'src/assets/images/profile/user-4.jpg';

import icon1 from 'src/assets/images/svgs/icon-account.svg';
import icon2 from 'src/assets/images/svgs/icon-inbox.svg';
import icon3 from 'src/assets/images/svgs/icon-tasks.svg';

import ddIcon1 from 'src/assets/images/svgs/icon-dd-employee.svg';
import ddIcon2 from 'src/assets/images/svgs/icon-dd-department.svg';
import ddIcon3 from 'src/assets/images/svgs/icon-dd-position.svg';



//
// Notifications dropdown
//
const notifications = [
  {
    avatar: img1,
    title: 'Roman Joined the Team!',
    subtitle: 'Congratulate him',
  },
  {
    avatar: img2,
    title: 'New message received',
    subtitle: 'Salma sent you new message',
  },
  {
    avatar: img3,
    title: 'New Payment received',
    subtitle: 'Check your earnings',
  },
  {
    avatar: img4,
    title: 'Jolly completed tasks',
    subtitle: 'Assign her new tasks',
  },
  {
    avatar: img1,
    title: 'Roman Joined the Team!',
    subtitle: 'Congratulate him',
  },
  {
    avatar: img2,
    title: 'New message received',
    subtitle: 'Salma sent you new message',
  },
  {
    avatar: img3,
    title: 'New Payment received',
    subtitle: 'Check your earnings',
  },
  {
    avatar: img4,
    title: 'Jolly completed tasks',
    subtitle: 'Assign her new tasks',
  },
];


const profile = [
  {
    href: '/change-password',
    title: 'Menu.ChangePassword',
    subtitle: 'Description.ChangePassword',
    icon: icon1,
  },
];


const appsLink = [
  {
    href: '/organization/employees',
    title: 'Menu.ManageEmployee',
    subtext: 'Description.ManageEmployee',
    avatar: ddIcon1,
  },
  {
    href: '/organization/departments',
    title: 'Menu.ManageDepartment',
    subtext: 'Description.ManageDepartment',
    avatar: ddIcon2,
  },
  {
    href: '/organization/positions',
    title: 'Menu.ManagePosition',
    subtext: 'Description.ManagePosition',
    avatar: ddIcon3,
  },
];

const pageLinks = [
  {
    href: '/auth/login',
    title: 'Authentication Design',
  },
  {
    href: '/auth/register',
    title: 'Register Now',
  },
  {
    href: '/404',
    title: '404 Error Page',
  },
  {
    href: '/user-profile',
    title: 'User Application',
  },
];

export { notifications, profile, pageLinks, appsLink };
