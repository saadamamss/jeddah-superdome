<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAppStore } from '@/stores/app.store'
import { validationRules } from '@/@core/utils/validators'
import axios from '@/plugins/axios'
import SearchableAutocomplete from '@/@core/components/global/SearchableAutocomplete.vue'

const props = defineProps({
  userId: Number,
})

const emit = defineEmits(['close', 'updated'])
const updateForm = ref(null)
const submitting = ref(false)
const isGettingUtilities = ref(false)
const loading = ref(false)
const appStore = useAppStore()
const roles = ref([])
const formState = reactive({
  firstname: '',
  lastname: '',
  parentCompanyIds: [],
  contractIds: [],
  supervisorIds: [],
  inspectorIds: [],
  email: '',
  phone: '',
  roleId: null,
  adminRoleId: null,
  city: '',
  bloodType: '',
  gender: '',
  nationality: '',
  saId: '',
})

const countryCode = ref('+966')
const contractSelectedItems = ref([])
const companySelectedItems = ref([])

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
    roleId: null,
    adminRoleId: null,
    city: '',
    bloodType: '',
    gender: '',
    nationality: '',
    saId: '',
  })
}

const updateUser = async () => {
  const { valid } = await updateForm.value.validate()
  if (!valid) return

  try {
    submitting.value = true
    const response = await axios.put(`/users/${props.userId}`, formState)

    if (response.data) {
      appStore.showSnackbar({
        message: 'تم تحديث بيانات المستخدم بنجاح',
        color: 'success',
      })

      resetFormState()
      emit('updated')
    }
  } catch (error) {
    logger.error('Error updating user:', error)
    appStore.showSnackbar({
      message: error.response?.data?.message || 'حدث خطأ أثناء تحديث بيانات المستخدم',
      color: 'error',
    })
  } finally {
    submitting.value = false
  }
}

const closeDialog = () => {
  emit('close')
}

const populateEditForm = (user) => {
  Object.assign(formState, {
    firstname: user.firstname || '',
    lastname: user.lastname || '',
    parentCompanyIds: user.parentCompanies?.map((c) => c.id) || [],
    contractIds: user.contracts?.map((c) => c.id) || [],
    supervisorIds: user.supervisors?.map((c) => c.id) || [],
    inspectorIds: user.inspectors?.map((c) => c.id) || [],
    email: user.email || '',
    phone: user.phone || '',
    roleId: user.role?.id || null,
    adminRoleId: user.adminRole?.id || null,
    city: user.city || '',
    bloodType: user.bloodType || '',
    gender: user.gender || '',
    nationality: user.nationality || '',
    saId: user.saId || '',
  })

  contractSelectedItems.value = user.contracts
  companySelectedItems.value = user.parentCompanies
}

const getUserDetails = async (userId) => {
  try {
    const { data } = await axios.get(`/users/${userId}`)
    if (data?.data?.user) {
      populateEditForm(data.data.user)
    }
  } catch (error) {
    logger.error('Error fetching user details:', error)
    appStore.showSnackbar({
      message: 'حدث خطأ أثناء جلب بيانات المستخدم',
      color: 'error',
    })
  } finally {
    loading.value = false
  }
}

const getUtilities = async () => {
  try {
    isGettingUtilities.value = true
    const response = await axios.get('/roles?perPage=1000')
    roles.value = response?.data?.data || []
  } catch (error) {
    logger.error('Error fetching ratings:', error)
  } finally {
    isGettingUtilities.value = false
  }
}

onMounted(() => {
  getUtilities()

  if (props.userId) {
    getUserDetails(props.userId)
  }
})
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
                bg-color="white"
                density="compact"
                hide-details
                placeholder="اسم المستخدم"
                :rules="[validationRules.required]"
                variant="outlined"
              />
            </v-col>
            <v-col cols="12" md="6">
              <label class="mb-2 d-block">اسم العائلة</label>
              <v-text-field
                v-model="formState.lastname"
                bg-color="white"
                density="compact"
                hide-details
                placeholder="اسم العائلة"
                :rules="[validationRules.required]"
                variant="outlined"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">رقم الهوية</label>
              <v-text-field
                v-model="formState.saId"
                bg-color="white"
                density="compact"
                hide-details
                placeholder="6127782187"
                :rules="[validationRules.required, validationRules.saId]"
                variant="outlined"
              />
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

            <v-col cols="12">
              <label class="mb-2 d-block">البريد الإلكتروني</label>
              <v-text-field
                v-model="formState.email"
                bg-color="white"
                density="compact"
                hide-details
                placeholder="البريد الإلكتروني"
                :rules="[validationRules.required, validationRules.email]"
                type="email"
                variant="outlined"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2 d-block">كلمة المرور</label>
              <v-text-field
                v-model="formState.password"
                bg-color="white"
                density="compact"
                hide-details
                placeholder="***********"
                type="password"
                variant="outlined"
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
                :selected-items="companySelectedItems"
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
                :selected-items="contractSelectedItems"
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
              <v-select
                v-model="formState.adminRoleId"
                item-title="name"
                item-value="id"
                :items="roles"
                :loading="isGettingFilters"
                required
              />
            </v-col>
          </v-row>

          <div class="d-flex gap-3 mt-6 justify-space-between">
            <v-btn color="error" max-width="150px" variant="outlined" @click="closeDialog"> إلغاء </v-btn>
            <v-btn class="bg-primary" color="primary" :loading="submitting" max-width="150px" type="submit">
              حفظ
            </v-btn>
          </div>
        </v-form>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
