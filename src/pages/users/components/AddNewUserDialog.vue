<script setup>
const emit = defineEmits(["close"]);

const formState = reactive({
  title: "",
  answer: "",
});

const weeklyHolidays = ref({
  saturday: false,
  sunday: false,
  monday: false,
  tuesday: false,
  wednesday: false,
  thursday: false,
  friday: false,
});

const closeDialog = () => {
  emit("close");
};
</script>

<template>
  <v-dialog width="625">
    <v-card class="py-4 px-2" rounded="lg">
      <v-card-title class="px-4 font-weight-bold">
        <div class="w-100 d-flex align-center justify-space-between">
          <h2 class="mb-0 text-h6 font-weight-bold">إضافة مدير جديد</h2>
          <v-btn icon variant="text" color="black" @click="closeDialog">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="mb-6">
        <v-row>
          <v-col cols="6">
            <label class="text-black">اسم المدير</label>
            <v-text-field
              v-model="formState.name"
              variant="outlined"
              class="mb-1"
              density="comfortable"
              placeholder="اسم المدير"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <label class="text-black">الرقم الوظيفي</label>
            <v-text-field
              v-model="formState.employee_number"
              variant="outlined"
              class="mb-1"
              density="comfortable"
              placeholder="ادخل الرقم"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <label class="text-black">البريد الالكتروني</label>
            <v-text-field
              v-model="formState.email"
              placeholder="البريد الالكتروني"
              variant="outlined"
              density="comfortable"
              class="mb-1"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <label class="text-black">منطقة العمل</label>
            <v-text-field
              v-model="formState.email"
              placeholder="منطقة العمل"
              variant="outlined"
              density="comfortable"
              class="mb-1"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <label class="text-black">رقم الجوال</label>
            <v-text-field
              v-model="formState.email"
              placeholder="رقم الجوال"
              variant="outlined"
              density="comfortable"
              class="mb-1"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <div
              class="d-flex align-center justify-space-between mt-5 mb-4 gap-2"
            >
              <v-text-field
                v-model="formState.start_time"
                label="بداية اوقات العمل"
                type="time"
                variant="outlined"
                density="comfortable"
              ></v-text-field>
              <v-text-field
                v-model="formState.end_time"
                label="نهاية اوقات العمل"
                type="time"
                variant="outlined"
                density="comfortable"
              ></v-text-field>
            </div>
          </v-col>
          <v-col cols="6">
            <label class="text-black">المسمى الوظيفي (الصلاحية)</label>
            <v-text-field
              v-model="formState.job_title"
              placeholder="المسمى الوظيفي"
              variant="outlined"
              density="comfortable"
              class="mb-1"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <label class="text-black">مدة التاخير المسموحة</label>
            <v-text-field
              v-model="formState.delay_time"
              type="time"
              placeholder="00:00 دقيقة"
              variant="outlined"
              density="comfortable"
              class="mb-1"
            ></v-text-field>
          </v-col>
        </v-row>

        <label class="d-inline-block mb-1 text-black"
          >ايام العطل الأسبوعيه</label
        >
        <div class="checkbox-container">
          <div class="checkBoxDay">
            <v-checkbox
              v-model="weeklyHolidays.saturday"
              label="السبت"
            ></v-checkbox>
          </div>
          <div class="checkBoxDay">
            <v-checkbox
              v-model="weeklyHolidays.sunday"
              label="الأحد"
            ></v-checkbox>
          </div>
          <div class="checkBoxDay">
            <v-checkbox
              v-model="weeklyHolidays.monday"
              label="الإثنين"
            ></v-checkbox>
          </div>
          <div class="checkBoxDay">
            <v-checkbox
              v-model="weeklyHolidays.tuesday"
              label="الثلاثاء"
            ></v-checkbox>
          </div>
          <div class="checkBoxDay">
            <v-checkbox
              v-model="weeklyHolidays.wednesday"
              label="الأربعاء"
            ></v-checkbox>
          </div>
          <div class="checkBoxDay">
            <v-checkbox
              v-model="weeklyHolidays.thursday"
              label="الخميس"
            ></v-checkbox>
          </div>
          <div class="checkBoxDay">
            <v-checkbox
              v-model="weeklyHolidays.friday"
              label="الجمعة"
            ></v-checkbox>
          </div>
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" variant="elevated" rounded="lg">
          <span class="d-inline-block px-4">إضافة مدير</span>
        </v-btn>
        <v-btn
          color="primary"
          variant="outlined"
          rounded="lg"
          @click="closeDialog"
        >
          <span class="d-inline-block px-4">الغاء</span>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
