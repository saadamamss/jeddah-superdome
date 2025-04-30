<script setup>
import { useAppStore } from "@/stores/app";
import { validationRules } from "@/@core/utils/validators";
import axios from "@/plugins/axios";
import SearchableAutocomplete from "@/@core/components/global/SearchableAutocomplete.vue";

const props = defineProps({
  userId: Number,
});

const emit = defineEmits(["close", "updated"]);
const updateForm = ref(null);
const submitting = ref(false);
const isGettingUtilities = ref(false);
const loading = ref(false);
const appStore = useAppStore();
const roles = ref([]);
const formState = reactive({
  firstname: "",
  lastname: "",
  parent_company_ids: [],
  contract_ids: [],
  supervisor_ids: [],
  inspector_ids: [],
  email: "",
  phone: "",
  role_id: null,
  admin_role_id: null,
  city: "",
  blood_type: "",
  gender: "",
  nationality: "",
  sa_id: "",
});

const countryCode = ref("+966");
const contractSelectedItems = ref([]);
const companySelectedItems = ref([]);

const resetFormState = () => {
  Object.assign(formState, {
    firstname: "",
    lastname: "",
    parent_company_ids: [],
    contract_ids: [],
    supervisor_ids: [],
    inspector_ids: [],
    email: "",
    phone: "",
    role_id: null,
    admin_role_id: null,
    city: "",
    blood_type: "",
    gender: "",
    nationality: "",
    sa_id: "",
  });
};

const updateUser = async () => {
  try {
    submitting.value = true;
    const response = await axios.put(`/users/${props.userId}`, formState);

    if (response.data) {
      appStore.showSnackbar({
        message: "تم تحديث بيانات المستخدم بنجاح",
        color: "success",
      });

      resetFormState();
      emit("updated");
    }
  } catch (error) {
    console.error("Error updating user:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء تحديث بيانات المستخدم",
      color: "error",
    });
  } finally {
    submitting.value = false;
  }
};

const closeDialog = () => {
  emit("close");
};

const populateEditForm = (user) => {
  Object.assign(formState, {
    firstname: user.firstname || "",
    lastname: user.lastname || "",
    parent_company_ids: user.parent_companies?.map((c) => c.id) || [],
    contract_ids: user.contracts?.map((c) => c.id) || [],
    supervisor_ids: user.supervisors?.map((c) => c.id) || [],
    inspector_ids: user.inspectors?.map((c) => c.id) || [],
    email: user.email || "",
    phone: user.phone || "",
    role_id: user.role?.id || null,
    admin_role_id: user.admin_role?.id || null,
    city: user.city || "",
    blood_type: user.blood_type || "",
    gender: user.gender || "",
    nationality: user.nationality || "",
    sa_id: user.sa_id || "",
  });

  contractSelectedItems.value = user.contracts;
  companySelectedItems.value = user.parent_companies;
};

const getUserDetails = async (userId) => {
  try {
    const { data } = await axios.get(`/users/${userId}`);
    if (data?.data?.user) {
      populateEditForm(data.data.user);
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

const getUtilities = async () => {
  try {
    isGettingUtilities.value = true;
    const response = await axios.get("/roles?per_page=1000");
    roles.value = response?.data?.data || [];
  } catch (error) {
    console.error("Error fetching ratings:", error);
  } finally {
    isGettingUtilities.value = false;
  }
};

onMounted(() => {
  getUtilities();

  if (props.userId) {
    getUserDetails(props.userId);
  }
});
</script>

<template>
  <v-dialog max-width="600px">
    <v-card class="pa-6">
      <v-card-title class="px-0 d-flex justify-space-between align-center">
        <h2 class="text-h5">تعديل البيانات</h2>
        <v-btn icon variant="text" @click="closeDialog">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-card-title>

      <v-card-subtitle class="px-0 text-medium-emphasis">
        يرجى عند تعديل بيانات المستخدم الضغط علي "حفظ" لإضافته إلى النظام
      </v-card-subtitle>

      <v-card-text class="px-0">
        <v-form ref="updateForm" @submit.prevent="updateUser">
          <v-row>
            <v-col cols="12" md="6">
              <label class="mb-2 d-block">الاسم الأول</label>
              <v-text-field
                v-model="formState.firstname"
                placeholder="اسم المستخدم"
                variant="outlined"
                density="compact"
                bg-color="white"
                hide-details
              ></v-text-field>
            </v-col>
            <v-col cols="12" md="6">
              <label class="mb-2 d-block">اسم العائلة</label>
              <v-text-field
                v-model="formState.lastname"
                placeholder="اسم العائلة"
                variant="outlined"
                density="compact"
                bg-color="white"
                hide-details
              ></v-text-field>
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">رقم الهوية</label>
              <v-text-field
                v-model="formState.sa_id"
                placeholder="6127782187"
                variant="outlined"
                density="compact"
                bg-color="white"
                hide-details
              ></v-text-field>
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">رقم الجوال</label>
              <div class="d-flex gap-2">
                <v-text-field
                  v-model="formState.phone"
                  placeholder="00"
                  variant="outlined"
                  density="compact"
                  bg-color="white"
                  hide-details
                  style="flex: 1"
                ></v-text-field>
                <v-select
                  v-model="countryCode"
                  :items="['+966']"
                  variant="outlined"
                  density="compact"
                  bg-color="white"
                  hide-details
                  style="max-width: 120px"
                ></v-select>
              </div>
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">البريد الإلكتروني</label>
              <v-text-field
                v-model="formState.email"
                type="email"
                placeholder="البريد الإلكتروني"
                variant="outlined"
                density="compact"
                bg-color="white"
                hide-details
              ></v-text-field>
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">كلمة المرور</label>
              <v-text-field
                v-model="formState.password"
                type="password"
                placeholder="***********"
                variant="outlined"
                density="compact"
                bg-color="white"
                hide-details
              ></v-text-field>
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">الشركة الرئيسية</label>
              <SearchableAutocomplete
                search-url="/companies?type=parent"
                v-model="formState.parent_company_ids"
                item-title="name"
                item-value="id"
                clearable
                multiple
                :return-object="false"
                :selected-items="companySelectedItems"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">الباقة</label>
              <SearchableAutocomplete
                search-url="/contracts"
                v-model="formState.contract_ids"
                item-title="name"
                item-value="id"
                clearable
                multiple
                :return-object="false"
                :selected-items="contractSelectedItems"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">المشرف</label>
              <SearchableAutocomplete
                search-url="/users?role_id=2"
                v-model="formState.supervisor_ids"
                item-title="label"
                :item-title-concat="['firstname', 'lastname']"
                item-value="id"
                clearable
                multiple
                :return-object="false"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">المراقب</label>
              <SearchableAutocomplete
                search-url="/users?role_id=3"
                v-model="formState.inspector_ids"
                item-title="label"
                :item-title-concat="['firstname', 'lastname']"
                item-value="id"
                clearable
                multiple
                :return-object="false"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block"> الدور الأساسي</label>
              <v-select
                v-model="formState.role_id"
                :items="[
                  { id: 1, title: 'مدير النظام' },
                  { id: 4, title: 'المشرف العام' },
                  { id: 2, title: 'المشرف' },
                  { id: 3, title: 'المراقب' },
                ]"
                item-title="title"
                item-value="id"
                :rules="[validationRules.required]"
                required
              ></v-select>
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">دور لوحة التحكم</label>
              <v-select
                v-model="formState.admin_role_id"
                :items="roles"
                item-title="name"
                item-value="id"
                required
                :loading="isGettingFilters"
              ></v-select>
            </v-col>
          </v-row>

          <div class="d-flex gap-3 mt-6 justify-space-between">
            <v-btn
              color="error"
              variant="outlined"
              max-width="150px"
              @click="closeDialog"
            >
              إلغاء
            </v-btn>
            <v-btn
              color="primary"
              type="submit"
              :loading="submitting"
              max-width="150px"
              class="bg-primary"
            >
              حفظ
            </v-btn>
          </div>
        </v-form>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
