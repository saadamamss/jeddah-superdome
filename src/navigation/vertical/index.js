import HomeIcon from "./icons/Home";
import SiteIcon from "./icons/Site";
import TeamIcon from "./icons/Team";
import PointIcon from "./icons/Point";
import RolesIcon from "./icons/Roles";
import UsersIcon from "./icons/Users";
import NotesIcon from "./icons/Notes";
import RatingIcon from "./icons/Rating";
import ContractsIcon from "./icons/Contracts";
import CompanyIcon from "./icons/Company";
import ShiftsIcon from "./icons/Shifts";
import QuestionnairesIcon from "./icons/Questionnaires";
import AttendanceIcon from "./icons/Attendance";

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
  // {
  //   title: "إدارة الشركات",
  //   to: { name: "companies" },
  //   icon: CompanyIcon,
  //   permission: true,
  // },
  {
    title: "⁠⁠إدارة الفرق الميدانية",
    to: { name: "teams" },
    icon: TeamIcon,
    permission: true,
  },
  {
    title: "⁠⁠إدارة التقييمات",
    to: { name: "rating" },
    icon: RatingIcon,
    permission: true,
  },
  // {
  //   title: "إدارة الباقات (التعاقدات)",
  //   to: { name: "contracts" },
  //   icon: ContractsIcon,
  //   permission: true,
  // },
  {
    title: "إدارة الاستبيانات",
    to: { name: "questionnaires" },
    icon: QuestionnairesIcon,
    permission: true,
  },
  // {
  //   title: "⁠⁠إدارة المراحل",
  //   to: { name: "main-sectors" },
  //   icon: PointIcon,
  //   permission: true,
  // },
  {
    title: "⁠⁠إدارة الملاحظات",
    to: { name: "notes" },
    icon: NotesIcon,
    permission: true,
  },
  {
    title: "سجل الحضور و الإنصراف",
    to: { name: "attendance" },
    icon: AttendanceIcon,
    permission: true,
  },
  {
    title: "إدارة المستخدمين",
    to: { name: "users" },
    icon: UsersIcon,
    permission: true,
  },
  // {
  //   title: "إدارة الأدوار والصلاحيات ",
  //   to: { name: "roles" },
  //   icon: RolesIcon,
  //   permission: true,
  // },
  // {
  //   title: "اداره الورديات",
  //   to: { name: "shifts" },
  //   icon: ShiftsIcon,
  //   permission: true,
  // },
  // --------------

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
