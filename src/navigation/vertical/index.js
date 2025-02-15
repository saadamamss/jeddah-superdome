import HomeIcon from "./icons/Home";
import SiteIcon from "./icons/Site";
import TeamIcon from "./icons/Team";
import PointIcon from "./icons/Point";
import RolesIcon from "./icons/Roles";
import UsersIcon from "./icons/Users";
import NotesIcon from "./icons/Notes";

export default [
  {
    title: "الصفحة الرئيسية",
    to: { name: "home" },
    icon: HomeIcon,
    permission: true,
  },
  {
    title: "إدارة المواقع",
    to: { name: "sites" },
    icon: SiteIcon,
    permission: true,
  },
  {
    title: "⁠⁠إدارة الفرق الميدانية",
    to: { name: "teams" },
    icon: TeamIcon,
    permission: true,
  },
  {
    title: "⁠⁠إدارة المحاور الرئيسية",
    to: { name: "main-sectors" },
    icon: PointIcon,
    permission: true,
  },
  {
    title: "إدارة الأدوار والصلاحيات ",
    to: { name: "roles" },
    icon: RolesIcon,
    permission: true,
  },

  {
    title: "إدارة المستخدمين",
    to: { name: "users" },
    icon: UsersIcon,
    permission: true,
  },
  {
    title: "⁠⁠إدارة الملاحظات",
    to: { name: "notes" },
    icon: NotesIcon,
    permission: true,
  },
  // {
  //   title: "مثال للاسترشاد",
  //   to: { name: "temp" },
  //   icon: HomeIcon,
  //   permission: true,
  // },

  // {
  //   title: "الإعدادت",
  //   icon: SettingsIcon,
  //   children: [
  //     { title: "Product_Cut", to: { name: "login" } },
  //     { title: "Product_Preparation", to: { name: "login" } },
  //     { title: "Product_Size", to: { name: "login" } },
  //   ],
  //   permission: true,
  // },
];
