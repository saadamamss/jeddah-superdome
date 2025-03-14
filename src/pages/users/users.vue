<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import UserCard from "@/@core/components/widgets/userCard";
import { ref, onMounted, watch } from "vue";
import axios from "@/plugins/axios";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);

// Data for users
const users = ref([]);
const totalUsers = ref(0);
const userDialog = ref(false);
const editUserDialog = ref(false);
const selectedUser = ref(null);
const deleteDialog = ref(false);
const userToDelete = ref(null);

// Form data for new/edit user
const userForm = ref({
  firstname: "",
  secondname: "",
  thirdname: "",
  lastname: "",
  email: "",
  phone: "",
  password: "",
  password_confirmation: "",
  role_id: null,
  city: "",
  blood_type: "",
  gender: "",
  nationality: "",
  sa_id: "",
  personal_image: "",
});

// Available roles for selection
const roles = ref([
  { id: 1, title: "ADMIN" },
  { id: 2, title: "SUPERVISOR" },
  { id: 3, title: "INSPECTOR" },
]);

// Blood types for selection
const bloodTypes = ["A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"];

// Fetch users with pagination and search
const fetchUsers = async () => {
  try {
    isLoading.value = true;
    const queryParams = new URLSearchParams({
      page: currentPage.value.toString(),
      per_page: perPage.value.toString(),
      search: search.value,
    });

    const response = await axios.get(`/users?${queryParams.toString()}`);
    if (response.data?.data) {
      users.value = response.data.data;
      console.log(response.data.data);
      // Update pagination data
      if (response.data.pagination) {
        totalUsers.value = response.data.pagination.i_total_objects;
        perPage.value = response.data.pagination.i_per_page;
        currentPage.value = response.data.pagination.i_current_page;
      }
    }
  } catch (error) {
    console.error("Error fetching users:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    users.value = [];
    totalUsers.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Add new user
const addUser = async () => {
  try {
    loading.value = true;
    const response = await axios.post("/users", userForm.value);

    if (response.data) {
      appStore.showSnackbar({
        message: "تم إضافة المستخدم بنجاح",
        color: "success",
      });
      resetForm();
      userDialog.value = false;
      await fetchUsers();
    }
  } catch (error) {
    console.error("Error adding user:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء إضافة المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Get user details
const getUserDetails = async (userId) => {
  try {
    loading.value = true;
    const response = await axios.get(`/users/${userId}`);
    if (response.data?.data?.user) {
      selectedUser.value = response.data.data.user;
      populateEditForm(response.data.data.user);
      editUserDialog.value = true;
    }
  } catch (error) {
    console.error("Error fetching user details:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Update user
const updateUser = async () => {
  try {
    loading.value = true;
    const response = await axios.put(
      `/users/${selectedUser.value.id}`,
      userForm.value
    );

    if (response.data) {
      appStore.showSnackbar({
        message: "تم تحديث بيانات المستخدم بنجاح",
        color: "success",
      });
      editUserDialog.value = false;
      await fetchUsers();
    }
  } catch (error) {
    console.error("Error updating user:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء تحديث بيانات المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Delete user confirmation
const confirmDelete = (userId) => {
  userToDelete.value = userId;
  deleteDialog.value = true;
};

// Delete user
const deleteUser = async () => {
  if (!userToDelete.value) return;

  try {
    loading.value = true;
    const response = await axios.delete(`/users/${userToDelete.value}`);

    if (response.data) {
      appStore.showSnackbar({
        message: "تم حذف المستخدم بنجاح",
        color: "success",
      });
      deleteDialog.value = false;
      userToDelete.value = null;
      await fetchUsers();
    }
  } catch (error) {
    console.error("Error deleting user:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Helper functions
const resetForm = () => {
  userForm.value = {
    firstname: "",
    secondname: "",
    thirdname: "",
    lastname: "",
    email: "",
    phone: "",
    password: "",
    password_confirmation: "",
    role_id: null,
    city: "",
    blood_type: "",
    gender: "",
    nationality: "",
    sa_id: "",
    personal_image: "",
  };
};

const populateEditForm = (user) => {
  userForm.value = {
    firstname: user.firstname || "",
    secondname: user.secondname || "",
    thirdname: user.thirdname || "",
    lastname: user.lastname || "",
    email: user.email || "",
    phone: user.phone || "",
    role_id: user.role?.id || null,
    city: user.city || "",
    blood_type: user.blood_type || "",
    gender: user.gender || "",
    nationality: user.nationality || "",
    sa_id: user.sa_id || "",
    personal_image: user.personal_image || "",
  };
};

// Watch for changes in search with debounce
const debouncedSearch = ref(null);
watch(search, (newValue) => {
  if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
  debouncedSearch.value = setTimeout(() => {
    currentPage.value = 1;
    fetchUsers();
  }, 500);
});

// Watch for page changes
watch(currentPage, () => {
  fetchUsers();
});

onMounted(() => {
  fetchUsers();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <div v-if="isLoading">
        <v-row>
          <v-col v-for="item in 4" :key="item" cols="6" sm="6" md="3">
            <VCard elevation="0">
              <v-card-text class="px-1 py-2">
                <v-skeleton-loader
                  class="mx-auto"
                  elevation="0"
                  max-width="400"
                  type="list-item-two-line, table-heading"
                ></v-skeleton-loader>
              </v-card-text>
            </VCard>
          </v-col>
        </v-row>
      </div>

      <div v-else>
        <v-container fluid class="px-0 rounded-lg">
          <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
            <v-card-title class="pt-4 pb-6">
              <div class="d-flex align-center justify-space-between gap-4">
                <h2 class="text-h5 font-weight-bold mb-4">إدارة المستخدمين</h2>
                <div class="d-flex align-center gap-2">
                  <v-text-field
                    v-model="search"
                    label="البحث عن مستخدم... "
                    rounded="lg"
                    clearable
                    height="48"
                    class="bg-white"
                    style="width: 320px; max-width: 100%"
                  >
                    <template v-slot:prepend-inner>
                      <v-icon>mdi-magnify</v-icon>
                    </template>
                  </v-text-field>

                  <VBtn
                    height="48"
                    elevation="0"
                    class="rounded-lg"
                    @click="userDialog = true"
                  >
                    إضافة مستخدم جديد
                  </VBtn>
                </div>
              </div>

              <v-row class="mt-6">
                <v-col
                  v-for="user in users"
                  :key="user.id"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                  class="pa-3"
                >
                  <UserCard
                    :user="user"
                    @edit="getUserDetails(user.id)"
                    @delete="confirmDelete(user.id)"
                  />
                </v-col>
                <v-col
                  v-if="!isLoading && users.length === 0"
                  cols="12"
                  class="text-center"
                >
                  <p class="text-medium-emphasis">لا يوجد مستخدمين</p>
                </v-col>
              </v-row>

              <!-- pagination -->
              <Pagination
                :total-items="totalUsers"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
            </v-card-title>
          </VCard>
        </v-container>
      </div>
    </v-container>

    <!-- Add User Dialog -->
    <v-dialog v-model="userDialog" max-width="600px">
      <v-card class="pa-4">
        <v-card-title class="text-h5 mb-4">إضافة مستخدم جديد</v-card-title>
        <v-card-text>
          <v-form @submit.prevent="addUser">
            <v-row>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الأول</label>
                <v-text-field
                  v-model="userForm.firstname"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الثاني</label>
                <v-text-field v-model="userForm.secondname"></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الثالث</label>
                <v-text-field v-model="userForm.thirdname"></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">اسم العائلة</label>
                <v-text-field
                  v-model="userForm.lastname"
                  required
                ></v-text-field>
              </v-col>
              <!-- <v-col cols="12" sm="6">
                <label class="mb-2 d-block">البريد الإلكتروني</label>
                <v-text-field
                  v-model="userForm.email"
                  type="email"
                  required
                ></v-text-field>
              </v-col> -->
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">رقم الهاتف</label>
                <v-text-field v-model="userForm.phone" required></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الدور</label>
                <v-select
                  v-model="userForm.role_id"
                  :items="roles"
                  item-title="title"
                  item-value="id"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">المدينة</label>
                <v-text-field v-model="userForm.city" required></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">فصيلة الدم</label>
                <v-select
                  v-model="userForm.blood_type"
                  :items="bloodTypes"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الجنس</label>
                <v-select
                  v-model="userForm.gender"
                  :items="['male', 'female']"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12">
                <label class="mb-2 d-block">الجنسية</label>
                <v-text-field
                  v-model="userForm.nationality"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <label class="mb-2 d-block">البريد الإلكتروني</label>
                <v-text-field
                  v-model="userForm.email"
                  type="email"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6" v-if="!selectedUser">
                <label class="mb-2 d-block">كلمة المرور</label>
                <v-text-field
                  v-model="userForm.password"
                  type="password"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6" v-if="!selectedUser">
                <label class="mb-2 d-block">تأكيد كلمة المرور</label>
                <v-text-field
                  v-model="userForm.password_confirmation"
                  type="password"
                  required
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" variant="text" @click="userDialog = false">
            إلغاء
          </v-btn>
          <v-btn color="primary" :loading="loading" @click="addUser">
            إضافة
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Edit User Dialog -->
    <v-dialog v-model="editUserDialog" max-width="600px">
      <v-card class="pa-4">
        <v-card-title class="text-h5 mb-4">تعديل بيانات المستخدم</v-card-title>
        <v-card-text>
          <v-form @submit.prevent="updateUser">
            <v-row>
              <v-col cols="12" sm="6">
                <v-text-field
                  v-model="userForm.firstname"
                  label="الاسم الأول"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  v-model="userForm.secondname"
                  label="الاسم الثاني"
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  v-model="userForm.thirdname"
                  label="الاسم الثالث"
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  v-model="userForm.lastname"
                  label="اسم العائلة"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  v-model="userForm.email"
                  label="البريد الإلكتروني"
                  type="email"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  v-model="userForm.phone"
                  label="رقم الهاتف"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-select
                  v-model="userForm.role_id"
                  :items="roles"
                  item-title="title"
                  item-value="id"
                  label="الدور"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  v-model="userForm.city"
                  label="المدينة"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-select
                  v-model="userForm.blood_type"
                  :items="bloodTypes"
                  label="فصيلة الدم"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12" sm="6">
                <v-select
                  v-model="userForm.gender"
                  :items="['male', 'female']"
                  label="الجنس"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  v-model="userForm.nationality"
                  label="الجنسية"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  v-model="userForm.sa_id"
                  label="رقم الهوية"
                  required
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" variant="text" @click="editUserDialog = false">
            إلغاء
          </v-btn>
          <v-btn color="primary" :loading="loading" @click="updateUser">
            حفظ
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="500px">
      <v-card>
        <v-card-title class="text-h5 pa-4">تأكيد الحذف</v-card-title>
        <v-card-text class="pa-4">
          هل أنت متأكد من حذف هذا المستخدم؟ لا يمكن التراجع عن هذا الإجراء.
        </v-card-text>
        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn
            color="error"
            variant="text"
            @click="deleteDialog = false"
            :disabled="loading"
          >
            إلغاء
          </v-btn>
          <v-btn
            color="primary"
            variant="flat"
            @click="deleteUser"
            :loading="loading"
          >
            تأكيد الحذف
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.v-text-field {
  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);

    &.v-field--focused {
      background-color: white;
    }
  }
}

.v-btn {
  text-transform: none;
  letter-spacing: 0;
  border-radius: 8px;
}
</style>
