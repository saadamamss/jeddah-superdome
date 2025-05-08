<script setup lang="ts">
import { ref } from "vue";
import { hasPermission } from "@core/utils/permissions";
import UnassignConfirmationDialog from "./UnassignConfirmationDialog.vue";
import { useRouter } from "vue-router";
import axios from "@/plugins/axios";

const router = useRouter();
const emit = defineEmits(["update"]);

const props = defineProps({
  user: {
    type: Object,
    required: true,
    default: () => ({
      fullname: "",
      firstname: "",
      lastname: "",
      role: { title: "مراقب" },
      sa_id: "",
      avatar_url: null,
      daily_visits_count: 0,
    }),
  },
  visitCount: {
    type: Number,
    default: 0,
  },
});

const showSupervisorsDt = (id) => {
  router.push(`/attributions/supervisors_dt/${id}`);
};

// const canEdit = hasPermission("teams.update");
// const canDelete = hasPermission("teams.delete");
const isAssigning = ref(false);
const isConfirmAssigningDialog = ref(false);

const unAssignInspector = async (user) => {
  try {
    isAssigning.value = true;

    console.log(user);

    await axios.post("un-assign-inspector-location", {
      assign_id: user.assign_id,
      inspector_id: user.id,
      // delete_all: true
    });

    isConfirmAssigningDialog.value = false;
    emit("update");
  } catch (err) {
    console.error(err);
  } finally {
    isAssigning.value = false;
  }
};
</script>

<template>
  <v-card elevation="0" class="rounded-lg bg-white pa-4">
    <div class="d-flex align-center gap-4">
      <v-avatar size="48">
        <v-img
          :src="user.avatar_url || '/imgs/person.svg'"
          alt="Observer"
        ></v-img>
      </v-avatar>
      <div class="flex-grow-1">
        <div class="d-flex justify-space-between align-center mb-1">
          <h3 class="text-subtitle-1 font-weight-medium mb-0">
            {{ user.fullname }}
          </h3>
          <div class="d-flex align-center gap-2">
            <button
              @click="showSupervisorsDt(user.id)"
              class="border rounded-lg d-flex align-center justify-center"
              style="width: 40px; height: 40px"
            >
              <svg
                width="15"
                height="12"
                viewBox="0 0 15 12"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M14.0003 6.00065C14.0003 5.79029 13.899 5.64819 13.6963 5.36397C12.7856 4.08694 10.4598 1.33398 7.33366 1.33398C4.20751 1.33398 1.88172 4.08695 0.971019 5.36397C0.768334 5.64819 0.666992 5.79029 0.666992 6.00065C0.666992 6.21101 0.768335 6.35311 0.97102 6.63733C1.88172 7.91436 4.20751 10.6673 7.33366 10.6673C10.4598 10.6673 12.7856 7.91436 13.6963 6.63733C13.899 6.35311 14.0003 6.21101 14.0003 6.00065ZM7.33366 4.00065C8.43823 4.00065 9.33366 4.89608 9.33366 6.00065C9.33366 7.10522 8.43823 8.00065 7.33366 8.00065C6.22909 8.00065 5.33366 7.10522 5.33366 6.00065C5.33366 4.89608 6.22909 4.00065 7.33366 4.00065Z"
                  fill="#B69265"
                />
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M7.33366 8.50065C5.95295 8.50065 4.83366 7.38136 4.83366 6.00065C4.83366 4.61994 5.95295 3.50065 7.33366 3.50065C8.71437 3.50065 9.83366 4.61994 9.83366 6.00065C9.83366 7.38136 8.71437 8.50065 7.33366 8.50065ZM5.83366 6.00065C5.83366 6.82908 6.50523 7.50065 7.33366 7.50065C8.16209 7.50065 8.83366 6.82908 8.83366 6.00065C8.83366 5.17222 8.16209 4.50065 7.33366 4.50065C6.50523 4.50065 5.83366 5.17222 5.83366 6.00065Z"
                  fill="#B69265"
                />
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M7.33366 0.833984C5.61995 0.833984 4.15096 1.58826 3.01386 2.47597C1.87575 3.36449 1.0345 4.41381 0.563932 5.07366L0.528453 5.12314C0.359683 5.35786 0.166992 5.62585 0.166992 6.00065C0.166992 6.37546 0.359683 6.64344 0.528454 6.87816L0.563933 6.92764C1.0345 7.58749 1.87575 8.63681 3.01386 9.52533C4.15096 10.413 5.61995 11.1673 7.33366 11.1673C9.04737 11.1673 10.5164 10.413 11.6535 9.52533C12.7916 8.63681 13.6328 7.58749 14.1034 6.92764L14.1389 6.87816C14.3076 6.64344 14.5003 6.37545 14.5003 6.00065C14.5003 5.62585 14.3076 5.35786 14.1389 5.12314L14.1034 5.07366C13.6328 4.41381 12.7916 3.36449 11.6535 2.47597C10.5164 1.58826 9.04737 0.833984 7.33366 0.833984ZM1.37811 5.65429C1.81824 5.03711 2.59524 4.07144 3.62923 3.26421C4.66424 2.45619 5.92122 1.83398 7.33366 1.83398C8.7461 1.83398 10.0031 2.45619 11.0381 3.26421C12.0721 4.07144 12.8491 5.03711 13.2892 5.65428C13.3981 5.80696 13.4499 5.88152 13.4808 5.94087C13.5004 5.97859 13.5004 5.98893 13.5003 5.99906L13.5003 6.00065L13.5003 6.00225C13.5004 6.01237 13.5004 6.02271 13.4808 6.06044C13.4499 6.11978 13.3981 6.19434 13.2892 6.34702C12.8491 6.96419 12.0721 7.92987 11.0381 8.73709C10.0031 9.54511 8.7461 10.1673 7.33366 10.1673C5.92122 10.1673 4.66424 9.54511 3.62923 8.73709C2.59524 7.92987 1.81824 6.96419 1.37811 6.34702C1.26922 6.19434 1.21741 6.11978 1.18651 6.06044C1.16688 6.02271 1.16693 6.01237 1.16699 6.00225L1.16699 6.00065L1.16699 5.99906C1.16693 5.98893 1.16688 5.97859 1.18651 5.94087C1.21741 5.88152 1.26922 5.80696 1.37811 5.65429Z"
                  fill="#B69265"
                />
              </svg>
            </button>
            <button
              v-if="hasPermission('assign_locations.unassign_inspector')"
              class="border rounded-lg d-flex align-center justify-center"
              style="width: 40px; height: 40px"
              @click="isConfirmAssigningDialog = true"
            >
              <svg
                width="15"
                height="16"
                viewBox="0 0 15 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M0.166016 7.99935C0.166016 11.9574 3.37464 15.166 7.33268 15.166C11.2907 15.166 14.4993 11.9574 14.4993 7.99935C14.4993 4.04131 11.2907 0.832682 7.33268 0.832682C3.37464 0.832682 0.166016 4.04131 0.166016 7.99935Z"
                  fill="#F53D6B"
                />
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M9.8041 5.52829C10.0644 5.78864 10.0644 6.21075 9.80407 6.47109L8.27539 7.99967L9.80407 9.52825C10.0644 9.7886 10.0644 10.2107 9.8041 10.4711C9.54376 10.7314 9.12165 10.7314 8.86129 10.4711L7.33255 8.94245L5.80407 10.4708C5.54371 10.7312 5.1216 10.7312 4.86126 10.4708C4.60092 10.2104 4.60094 9.78833 4.86129 9.52799L6.38971 7.99967L4.86129 6.47136C4.60094 6.21102 4.60092 5.78891 4.86126 5.52855C5.1216 5.26819 5.54371 5.26818 5.80407 5.52852L7.33255 7.0569L8.86129 5.52825C9.12165 5.26791 9.54376 5.26793 9.8041 5.52829Z"
                  fill="#F53D6B"
                />
              </svg>
            </button>
          </div>
        </div>
        <!-- <p class="text-body-2 text-medium-emphasis mb-0">
          {{ user.role?.title || "مراقب" }}
        </p> -->
      </div>
    </div>

    <div class="mt-4 bg-light-gray-1 rounded-lg pa-4">
      <div class="d-flex justify-space-between">
        <div>
          <div class="text-body-2 text-medium-emphasis">رقم الهوية</div>
          <div class="text-body-1">{{ user.sa_id }}</div>
        </div>

        <div>
          <div class="text-body-2 text-medium-emphasis">عدد مرات الزيارة</div>
          <div class="text-body-1 text-center">{{ visitCount }}</div>
        </div>
      </div>
    </div>

    <UnassignConfirmationDialog
      v-model="isConfirmAssigningDialog"
      @close="isConfirmAssigningDialog = false"
      @confirm="unAssignInspector(user)"
      :isProcessing="isAssigning"
    />
  </v-card>
</template>

<style lang="scss" scoped>
.bg-light-gray-1 {
  background-color: rgb(250, 250, 250);
}

.v-avatar {
  border: 2px solid rgb(var(--v-theme-warning));
}

.border {
  border: 1px solid #e9ebec;
}
</style>
