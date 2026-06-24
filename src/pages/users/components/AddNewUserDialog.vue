<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAppStore } from '@/stores/app.store'
import { validationRules } from '@/@core/utils/validators'
import axios from '@/plugins/axios'
import SearchableAutocomplete from '@/@core/components/global/SearchableAutocomplete.vue'

const emit = defineEmits(['close', 'added'])
const addUserForm = ref(null)
const submitting = ref(false)
const isGettingUtilities = ref(false)
const appStore = useAppStore()
const formState = reactive({
  firstname: '',
  lastname: '',
  parentCompanyIds: [],
  contractIds: [],
  supervisorIds: [],
  inspectorIds: [],
  email: '',
  phone: '',
  password: '',
  passwordConfirmation: '',
  roleId: null,
  adminRoleId: null,
  city: '',
  bloodType: '',
  gender: '',
  nationality: '',
  saId: '',
})

const countryCode = ref('+966')

const resetFormState = () => {
  Object.assign(formState, {
    firstname: '',
    lastname: '',
    parentCompanyIds: [],
    contractIds: [],
    supervisorIds: [],
    inspectorIds: [],
    email: '',
    phone: '',
    password: '',
    passwordConfirmation: '',
    roleId: null,
    adminRoleId: null,
    city: '',
    bloodType: '',
    gender: '',
    nationality: '',
    saId: '',
  })
}

const addUser = async () => {
  const { valid } = await addUserForm.value.validate()

  if (!valid) {
    appStore.showSnackbar({
      message: 'يرجى التحقق من صحة البيانات المدخلة',
      color: 'error',
    })
    return
  }

  try {
    submitting.value = true
    const response = await axios.post('/users', formState)

    if (response.data) {
      appStore.showSnackbar({
        message: 'تم إضافة المستخدم بنجاح',
        color: 'success',
      })

      resetFormState()
      emit('added')
    }
  } catch (error) {
    logger.error('Error adding user:', error)
    appStore.showSnackbar({
      message: error.response?.data?.message || 'حدث خطأ أثناء إضافة المستخدم',
      color: 'error',
    })
  } finally {
    submitting.value = false
  }
}

const closeDialog = () => {
  emit('close')
}
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
              <v-text-field v-model="formState.firstname" required :rules="[validationRules.required]" />
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
              <v-text-field v-model="formState.lastname" required :rules="[validationRules.required]" />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">رقم الجوال</label>
              <div class="d-flex gap-2">
                <v-text-field
                  v-model="formState.phone"
                  bg-color="white"
                  density="compact"
                  hide-details
                  placeholder="00"
                  :rules="[validationRules.required, validationRules.phone]"
                  style="flex: 1"
                  type="number"
                  variant="outlined"
                />
                <v-select
                  v-model="countryCode"
                  bg-color="white"
                  density="compact"
                  hide-details
                  :items="['+966']"
                  style="max-width: 120px"
                  variant="outlined"
                />
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
                    v-model="formState.bloodType"
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
                required
                :rules="[validationRules.required, validationRules.email]"
                type="email"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">رقم الهوية</label>
              <v-text-field
                v-model="formState.saId"
                placeholder="مثال: 1234567890"
                required
                :rules="[validationRules.required, validationRules.saId]"
                type="number"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">كلمة المرور</label>
              <v-text-field
                v-model="formState.password"
                required
                :rules="[validationRules.required, validationRules.password]"
                type="password"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">تأكيد كلمة المرور</label>
              <v-text-field
                v-model="formState.passwordConfirmation"
                required
                :rules="[
                  validationRules.required,
                  validationRules.passwordMatch(formState.passwordConfirmation, formState.password),
                ]"
                type="password"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">الشركة الرئيسية</label>
              <SearchableAutocomplete
                v-model="formState.parentCompanyIds"
                clearable
                item-title="name"
                item-value="id"
                multiple
                :return-object="false"
                search-url="/companies?type=parent"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">الباقة</label>
              <SearchableAutocomplete
                v-model="formState.contractIds"
                clearable
                item-title="name"
                item-value="id"
                multiple
                :return-object="false"
                search-url="/contracts"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">المشرف</label>
              <SearchableAutocomplete
                v-model="formState.supervisorIds"
                clearable
                item-title="label"
                :item-title-concat="['firstname', 'lastname']"
                item-value="id"
                multiple
                :return-object="false"
                search-url="/users?roleId=2"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">المراقب</label>
              <SearchableAutocomplete
                v-model="formState.inspectorIds"
                clearable
                item-title="label"
                :item-title-concat="['firstname', 'lastname']"
                item-value="id"
                multiple
                :return-object="false"
                search-url="/users?roleId=3"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block"> الدور الأساسي</label>
              <v-select
                v-model="formState.roleId"
                item-title="title"
                item-value="id"
                :items="[
                  { id: 1, title: 'مدير النظام' },
                  { id: 4, title: 'المشرف العام' },
                  { id: 2, title: 'المشرف' },
                  { id: 3, title: 'المراقب' },
                ]"
                required
                :rules="[validationRules.required]"
              />
            </v-col>
            <v-col cols="12">
              <label class="mb-2 d-block">دور لوحة التحكم</label>
              <SearchableAutocomplete
                v-model="formState.adminRoleId"
                clearable
                item-title="name"
                item-value="id"
                :loading="isGettingUtilities"
                required
                :return-object="false"
                search-url="/roles"
              />
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn class="bg-red" color="error" type="button" @click="closeDialog()"> إلغاء </v-btn>
        <v-btn class="px-6 bg-primary text-white" color="primary" :loading="submitting" @click="addUser">
          إضافة مستخدم
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
