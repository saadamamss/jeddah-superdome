<script setup>
import { useAppStore } from "@/stores/app";
import { validationRules } from "@/@core/utils/validators";
import axios from "@/plugins/axios";
import SearchableAutocomplete from "@/@core/components/global/SearchableAutocomplete.vue";

const emit = defineEmits(["close", "added"]);
const addUserForm = ref(null);
const submitting = ref(false);
const isGettingUtilities = ref(false);
const appStore = useAppStore();
const roles = ref([]);
const formState = reactive({
  firstname: "",
  lastname: "",
  parent_company_ids: [],
  contract_ids: [],
  email: "",
  phone: "",
  password: "",
  password_confirmation: "",
  role_id: null,
  admin_role_id: null,
  city: "",
  blood_type: "",
  gender: "",
  nationality: "",
  sa_id: "",
});

const countryCode = ref("+966");

const resetFormState = () => {
  Object.assign(formState, {
    firstname: "",
    lastname: "",
    parent_company_ids: [],
    contract_ids: [],
    email: "",
    phone: "",
    password: "",
    password_confirmation: "",
    role_id: null,
    admin_role_id: null,
    city: "",
    blood_type: "",
    gender: "",
    nationality: "",
    sa_id: "",
  });
};

const addUser = async () => {
  const { valid } = await addUserForm.value.validate();

  if (!valid) {
    appStore.showSnackbar({
      message: "يرجى التحقق من صحة البيانات المدخلة",
      color: "error",
    });
    return;
  }

  try {
    submitting.value = true;
    const response = await axios.post("/users", formState);

    if (response.data) {
      appStore.showSnackbar({
        message: "تم إضافة المستخدم بنجاح",
        color: "success",
      });

      resetFormState();
      emit("added");
    }
  } catch (error) {
    console.error("Error adding user:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء إضافة المستخدم",
      color: "error",
    });
  } finally {
    submitting.value = false;
  }
};

const closeDialog = () => {
  emit("close");
};
</script>

<template>
  <v-dialog max-width="600px">
    <v-card class="pa-4">
      <v-card-title class="text-h5 mb-4">إضافة مستخدم جديد</v-card-title>
      <v-card-text>
        <v-form ref="addUserForm" @submit.prevent="addUser">
          <v-row>
            <v-col cols="12" md="6">
              <label class="mb-2 d-block">الاسم الأول</label>
              <v-text-field
                v-model="formState.firstname"
                :rules="[validationRules.required]"
                required
              ></v-text-field>
            </v-col>
            <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الاسم الثاني</label>
                  <v-text-field v-model="formState.secondname"></v-text-field>
                </v-col> -->
            <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الاسم الثالث</label>
                  <v-text-field v-model="formState.thirdname"></v-text-field>
                </v-col> -->
            <v-col cols="12" md="6">
              <label class="mb-2 d-block">اسم العائلة</label>
              <v-text-field
                v-model="formState.lastname"
                :rules="[validationRules.required]"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">رقم الجوال</label>
              <div class="d-flex gap-2">
                <v-text-field
                  v-model="formState.phone"
                  type="number"
                  :rules="[validationRules.required, validationRules.phone]"
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

            <!-- <v-col cols="12">
                  <label class="mb-2 d-block">المدينة</label>
                  <v-text-field
                    v-model="formState.city"
                    :rules="[validationRules.required]"
                    required
                  ></v-text-field>
                </v-col> -->
            <!-- <v-col cols="12">
                  <label class="mb-2 d-block">فصيلة الدم</label>
                  <v-select
                    v-model="formState.blood_type"
                    :items="bloodTypes"
                    :rules="[validationRules.required]"
                    required
                  ></v-select>
                </v-col> -->
            <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الجنس</label>
                  <v-select
                    v-model="formState.gender"
                    :items="[
                      { title: 'ذكر', value: 'male' },
                      { title: 'أنثى', value: 'female' },
                    ]"
                    :rules="[validationRules.required]"
                    required
                  ></v-select>
                </v-col> -->
            <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الجنسية</label>
                  <v-text-field
                    v-model="formState.nationality"
                    :rules="[validationRules.required]"
                    required
                  ></v-text-field>
                </v-col> -->
            <v-col cols="12">
              <label class="mb-2 d-block">البريد الإلكتروني</label>
              <v-text-field
                v-model="formState.email"
                type="email"
                :rules="[validationRules.required, validationRules.email]"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">رقم الهوية</label>
              <v-text-field
                v-model="formState.sa_id"
                type="number"
                :rules="[validationRules.required, validationRules.saId]"
                placeholder="مثال: 1234567890"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">كلمة المرور</label>
              <v-text-field
                v-model="formState.password"
                type="password"
                :rules="[validationRules.required, validationRules.password]"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">تأكيد كلمة المرور</label>
              <v-text-field
                v-model="formState.password_confirmation"
                type="password"
                :rules="[
                  validationRules.required,
                  validationRules.passwordMatch(
                    formState.password_confirmation,
                    formState.password
                  ),
                ]"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">الشركة الرئيسية</label>
              <SearchableAutocomplete
                search-url="/companies"
                v-model="formState.parent_company_ids"
                item-title="name"
                item-value="id"
                clearable
                multiple
                :return-object="false"
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
              <SearchableAutocomplete
                search-url="/roles"
                v-model="formState.admin_role_id"
                item-title="name"
                item-value="id"
                required
                :loading="isGettingUtilities"
                clearable
                :return-object="false"
              ></SearchableAutocomplete>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
          type="button"
          class="bg-red"
          color="error"
          @click="closeDialog()"
        >
          إلغاء
        </v-btn>
        <v-btn
          color="primary"
          :loading="submitting"
          @click="addUser"
          class="px-6 bg-primary text-white"
        >
          إضافة مستخدم
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
