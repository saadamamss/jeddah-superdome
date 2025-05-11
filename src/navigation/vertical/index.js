import Airplane from "@/components/icons/airplane.vue";
import HomeIcon from "@/components/icons/home.vue";
import Notifications from "@/components/icons/notifications.vue";
import Realstate from "@/components/icons/realstate.vue";
import Usergroup from "@/components/icons/usergroup.vue";
import Userstar from "@/components/icons/userstar.vue";

export default [
  {
    heading: "الطلبات الأساسية",
    children: [
      {
        title: "الصفحة الرئيسية",
        to: { name: "home" },
        icon: HomeIcon,
        permission: true,
      },
      {
        title: "إدارة الشركات",
        to: { name: "companies" },
        icon: Realstate,
        permission: true,
      },
      {
        title: "إدارة الحجاج",
        to: { name: "manage-pilgrims" },
        icon: Usergroup,
        permission: true,
      },
      {
        title: "جداول الرحلات",
        to: { name: "flights-table" },
        icon: Airplane,
        permission: true,
      },
    ],
  },
  {
    divider: true,
  },
  //
  {
    heading: "خدمات النظام",
    children: [
      {
        title: "إدارة المستخدمين",
        to: { name: "users" },
        icon: Userstar,
        permission: true,
      },
      {
        title: "إدارة الإشعارات",
        to: { name: "notifications" },
        icon: Notifications,
        permission: true,
      },
    ],
  },

  // {
  //   title: "إدارة المواقع",
  //   to: { name: "sites" },
  //   icon: SiteIcon,
  //   permission: "locations.view",
  // },
  // {
  //   title: "إدارة الشركات",
  //   to: { name: "companies" },
  //   icon: CompanyIcon,
  //   permission: "companies.view",
  // },
  // {
  //   title: "⁠⁠إدارة الفرق الميدانية",
  //   to: { name: "teams" },
  //   icon: TeamIcon,
  //   permission: "teams.view",
  // },
  // {
  //   title: "⁠⁠إدارة التقييمات",
  //   to: { name: "rating" },
  //   icon: RatingIcon,
  //   permission: "ratings.view",
  // },
  // {
  //   title: "إدارة الباقات (التعاقدات)",
  //   to: { name: "contracts" },
  //   icon: ContractsIcon,
  //   permission: "contracts.view",
  // },
  // {
  //   title: "الإسنادات",
  //   to: { name: "attributions" },
  //   icon: AttributionsIcon,
  //   permission: "assign_locations.view",
  // },
  // {
  //   title: "إدارة الاستبيانات",
  //   to: { name: "questionnaires" },
  //   icon: QuestionnairesIcon,
  //   permission: "questionnaires.view",
  // },
  // {
  //   title: "إدارة التنبيهات",
  //   to: { name: "notifications" },
  //   icon: NotificationIcon,
  //   permission: "admin_notifications.view",
  // },
  // {
  //   title: "⁠⁠إدارة المراحل",
  //   to: { name: "phases" },
  //   icon: PointIcon,
  //   permission: "imtithal_phases.view",
  // },
  // {
  //   title: "⁠⁠إدارة الملاحظات",
  //   to: { name: "notes" },
  //   icon: NotesIcon,
  //   permission: "notes.view",
  // },
  // {
  //   title: "إدارة التكاملات",
  //   to: { name: "integrations" },
  //   icon: IntegrationIcon,
  //   permission: "integrations.view",
  // },
  // {
  //   title: "سجل الحضور و الإنصراف",
  //   to: { name: "attendance" },
  //   icon: AttendanceIcon,
  //   permission: "attendances.view",
  // },
  // {
  //   title: "إدارة المستخدمين",
  //   to: { name: "users" },
  //   icon: UsersIcon,
  //   permission: "users.view",
  // },
  // {
  //   title: "إدارة الأدوار والصلاحيات ",
  //   to: { name: "roles" },
  //   icon: RolesIcon,
  //   permission: "permissions.view",
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
  //   permission: "settings.view",
  // },
];
