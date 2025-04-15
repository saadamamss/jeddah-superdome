<script setup>
import UserWidget from "./UserWidget.vue";
import MapDisplay from "@/components/maps/MapDisplay.vue";
import { useAppStore } from "@/stores/app";
import { ref } from "vue";

const appStore = useAppStore();
const props = defineProps({
  user: {
    type: Object,
    required: true,
  },
  onClick: {
    type: Function,
    default: () => {},
  },
});

const mapDialog = ref(false);
const confirmDialog = ref(false);
const deleteLoading = ref(false);

const openMapDialog = () => {
  mapDialog.value = true;
};

const emit = defineEmits(["delete", "edit"]);

const handleDelete = () => {
  confirmDialog.value = true;
};

const confirmDelete = () => {
  deleteLoading.value = true;
  emit("delete", props.user.id);
  confirmDialog.value = false;
};

const handleEdit = () => {
  emit("edit", props.user.id);
};

const handleMapError = (error) => {
  console.error("Map error:", error);
  appStore.showSnackbar({
    message: "حدث خطأ أثناء تحميل الخريطة",
    color: "error",
  });
};
</script>

<template>
  <v-card elevation="0" class="user-card rounded-lg">
    <v-card-text class="pa-4">
      <div class="border-b mb-4 border-color-light-white pb-4 cursor-pointer">
        <div class="d-flex align-center gap-1 justify-space-between">
          <div class="d-flex align-start flex-column">
            <UserWidget :user="user" class="pa-0" />

            <div class="font-weight-medium ms-12" style="font-size: 12px">
              {{ user.role.title }}
            </div>
          </div>
        </div>
      </div>

      <div class="user-info d-flex flex-column ga-4">
        <div class="d-flex justify-space-between align-center">
          <!-- <div class="d-flex align-start gap-2 cursor-pointer" @click="onClick">
            <svg
              width="12"
              height="16"
              viewBox="0 0 12 16"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M5.99951 12.0007C6.31218 12.0007 6.61284 11.8793 6.8375 11.6633C6.99484 11.512 7.15552 11.3593 7.31885 11.2047C9.29685 9.32799 11.5522 7.18932 10.3108 4.20332C9.58284 2.45332 7.83618 1.33398 5.99951 1.33398C4.16285 1.33398 2.41617 2.45332 1.68884 4.20332C0.452171 7.17732 2.68217 9.29999 4.65017 11.1733C4.8235 11.3387 4.99486 11.5013 5.16153 11.6633C5.38619 11.8793 5.68685 12.0007 5.99951 12.0007ZM5.99951 7.66732C6.92018 7.66732 7.66618 6.92132 7.66618 6.00065C7.66618 5.07998 6.92018 4.33398 5.99951 4.33398C5.07885 4.33398 4.33285 5.07998 4.33285 6.00065C4.33285 6.92132 5.07885 7.66732 5.99951 7.66732Z"
                fill="#B69265"
              />
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M5.99935 8.16732C4.80468 8.16732 3.83268 7.19532 3.83268 6.00065C3.83268 4.80598 4.80468 3.83398 5.99935 3.83398C7.19402 3.83398 8.16602 4.80598 8.16602 6.00065C8.16602 7.19532 7.19402 8.16732 5.99935 8.16732ZM5.99935 4.83398C5.35602 4.83398 4.83268 5.35732 4.83268 6.00065C4.83268 6.64398 5.35602 7.16732 5.99935 7.16732C6.64268 7.16732 7.16602 6.64398 7.16602 6.00065C7.16602 5.35732 6.64268 4.83398 5.99935 4.83398Z"
                fill="#B69265"
              />
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M4.81465 12.024C5.13465 12.3313 5.55532 12.5007 5.99932 12.5007C6.44332 12.5007 6.86399 12.3313 7.18399 12.0233C7.33999 11.8733 7.49998 11.7213 7.66198 11.568C9.65928 9.67335 12.1459 7.31452 10.772 4.01131C9.98198 2.11065 8.06332 0.833984 5.99932 0.833984C3.93532 0.833984 2.01732 2.11065 1.22732 4.01131C-0.142016 7.30265 2.32398 9.64999 4.30531 11.536L4.32448 11.5543C4.48995 11.712 4.65354 11.868 4.81465 12.024ZM2.15066 4.39531C2.78732 2.86331 4.33399 1.83398 5.99932 1.83398C7.66466 1.83398 9.21199 2.86331 9.84932 4.39531C10.9587 7.06331 8.93333 8.98464 6.974 10.8426C6.93684 10.878 6.89975 10.9132 6.86276 10.9484C6.73716 11.0677 6.61272 11.1859 6.49066 11.3033C6.358 11.4307 6.18332 11.5007 5.99932 11.5007C5.81532 11.5007 5.64065 11.43 5.50932 11.304C5.34132 11.1413 5.16933 10.9773 4.99467 10.8113C3.0534 8.96338 1.04536 7.05188 2.15066 4.39531Z"
                fill="#B69265"
              />
              <path
                d="M7.81131 15.1673H4.18728C2.03328 15.1673 0.951964 15.1673 0.419964 14.378C0.368631 14.3013 0.323949 14.222 0.287282 14.142C-0.119178 13.2718 0.535303 12.3389 1.44059 11.0486L1.44197 11.0466C1.60064 10.8206 1.91262 10.766 2.13862 10.9247C2.36462 11.0833 2.41929 11.3953 2.26062 11.6213C1.54129 12.6467 1.04329 13.398 1.19529 13.7227C1.21129 13.7573 1.22928 13.7887 1.24994 13.82C1.48394 14.1673 2.55927 14.1673 4.18725 14.1673H7.81131C9.43928 14.1673 10.5146 14.1673 10.7493 13.8193C10.77 13.7887 10.788 13.7573 10.802 13.726C10.9553 13.398 10.4573 12.6467 9.73797 11.6213C9.5793 11.3953 9.63397 11.0833 9.85997 10.9247C10.086 10.766 10.398 10.8206 10.5566 11.0466C11.4626 12.338 12.1173 13.2713 11.71 14.1453C11.6746 14.222 11.6306 14.3013 11.5793 14.3773C11.0466 15.1673 9.96531 15.1673 7.81131 15.1673Z"
                fill="#B69265"
              />
            </svg>

            <div @click="openMapDialog">
              <div class="text-body-2 text-medium-emphasis">الموقع</div>
              <div class="text-body-2 font-weight-medium">
                {{ user.city }}
              </div>
            </div>
          </div> -->
        </div>
        <div
          v-if="user.sa_id"
          class="d-flex justify-space-between align-center"
        >
          <div class="d-flex align-start gap-2">
            <svg
              width="17"
              height="16"
              viewBox="0 0 17 16"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                d="M9.49935 2.66415C9.50038 2.38801 9.72507 2.16499 10.0012 2.16602C10.8774 2.16928 11.5934 2.18549 12.1718 2.28166C12.7625 2.37988 13.2574 2.56849 13.6598 2.9497C14.1145 3.38051 14.3149 3.92797 14.409 4.59048C14.4994 5.22758 14.4994 6.03842 14.4994 7.04757V10.2829C14.4994 11.292 14.4994 12.1029 14.409 12.74C14.3149 13.4025 14.1145 13.95 13.6598 14.3808C13.2089 14.808 12.6422 14.9929 11.9562 15.0803C11.2892 15.1653 10.438 15.1652 9.36814 15.1652H9.36813H7.29723H7.29722C6.22737 15.1652 5.37621 15.1653 4.70921 15.0803C4.02312 14.9929 3.45649 14.808 3.00556 14.3808C2.55082 13.95 2.35042 13.4025 2.2564 12.74C2.16599 12.1029 2.166 11.2921 2.16602 10.2829V10.2829V7.04755V7.04754C2.166 6.0384 2.16599 5.22758 2.2564 4.59048C2.35042 3.92797 2.55082 3.38051 3.00556 2.9497C3.40796 2.56849 3.9029 2.37988 4.49358 2.28166C5.07196 2.18549 5.78798 2.16928 6.66415 2.16602C6.94029 2.16499 7.16498 2.38801 7.16601 2.66415L6.66788 3.86479H9.99749L9.49935 2.66415Z"
                fill="#B69265"
              />
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M8.28162 0.833029C8.01803 0.832856 7.80659 0.832716 7.61453 0.876921C7.17241 0.978676 6.78274 1.25442 6.56529 1.65415C6.4703 1.82879 6.42361 2.03171 6.37148 2.25836L6.29911 2.57137C6.24699 2.79664 6.19922 3.00306 6.17829 3.17625C6.15582 3.36221 6.15429 3.57708 6.25168 3.79177C6.3512 4.01116 6.51674 4.19041 6.71883 4.31314C6.90893 4.4286 7.11495 4.4667 7.30745 4.48357C7.49199 4.49974 7.71918 4.49973 7.97944 4.49972H8.68531C8.94557 4.49973 9.17276 4.49974 9.3573 4.48357C9.5498 4.4667 9.75582 4.4286 9.94592 4.31314C10.148 4.19041 10.3135 4.01116 10.4131 3.79177C10.5105 3.57708 10.5089 3.36221 10.4865 3.17625C10.4655 3.00306 10.4178 2.79665 10.3656 2.57138L10.2933 2.25836C10.2411 2.03172 10.1944 1.82879 10.0995 1.65415C9.88201 1.25442 9.49234 0.978676 9.05022 0.876921C8.85815 0.832716 8.64672 0.832856 8.38312 0.833029H8.28162ZM7.83882 1.85144C7.9051 1.83619 7.99259 1.83305 8.33237 1.83305C8.67216 1.83305 8.75964 1.83619 8.82593 1.85144C9.01402 1.89473 9.15227 2.00563 9.22101 2.132C9.24196 2.17051 9.25962 2.22719 9.33046 2.53348L9.386 2.77357C9.44525 3.02974 9.47999 3.18293 9.49367 3.29621C9.5 3.34855 9.49935 3.37565 9.49847 3.38658C9.48603 3.40979 9.46328 3.43628 9.42662 3.45855L9.42591 3.45892C9.42524 3.45925 9.424 3.45984 9.42204 3.46065C9.41812 3.46225 9.41088 3.46488 9.39904 3.46798C9.37411 3.47451 9.33427 3.48175 9.27 3.48739C9.13444 3.49927 8.95168 3.49972 8.66444 3.49972H8.0003C7.71307 3.49972 7.53031 3.49927 7.39475 3.48739C7.33048 3.48175 7.29064 3.47451 7.26571 3.46798C7.25387 3.46488 7.24662 3.46225 7.24271 3.46065C7.24158 3.46019 7.24069 3.45979 7.24001 3.45948C7.23951 3.45925 7.23912 3.45906 7.23883 3.45892L7.23794 3.45844C7.20128 3.43617 7.17872 3.40979 7.16628 3.38658C7.16539 3.37565 7.16475 3.34855 7.17107 3.29621C7.18476 3.18293 7.2195 3.02974 7.27875 2.77357L7.33428 2.53349C7.40513 2.22719 7.42279 2.17051 7.44374 2.132C7.51248 2.00563 7.65072 1.89473 7.83882 1.85144ZM7.16696 3.39228C7.16695 3.39229 7.16699 3.39245 7.16696 3.39228V3.39228Z"
                fill="#B69265"
              />
              <path
                d="M8.32797 6.16634C7.32236 6.16634 6.52982 6.99984 6.52982 7.99967C6.52982 8.99951 7.32236 9.83301 8.32797 9.83301C9.33358 9.83301 10.1261 8.99951 10.1261 7.99967C10.1261 6.99984 9.33358 6.16634 8.32797 6.16634Z"
                fill="#B69265"
              />
              <path
                d="M10.1835 13.1663C10.7259 13.1663 11.1657 12.7266 11.1657 12.1841C11.1657 11.9484 11.0807 11.6984 10.889 11.5098C9.48694 10.1302 7.16177 10.1982 5.78696 11.5034C5.58808 11.6922 5.49902 11.9469 5.49902 12.1876C5.49902 12.7282 5.93721 13.1663 6.47775 13.1663H10.1835Z"
                fill="#B69265"
              />
            </svg>

            <div>
              <div class="text-body-2 text-medium-emphasis">رقم الهوية</div>
              <div class="text-body-2 font-weight-medium">
                {{ user.sa_id }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Map Dialog -->
      <v-dialog v-model="mapDialog" max-width="800">
        <v-card>
          <v-card-title class="d-flex justify-space-between align-center pa-4">
            <span>موقع {{ user.firstname }} {{ user.lastname }}</span>
            <v-btn icon @click="mapDialog = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </v-card-title>
          <v-card-text class="pa-0">
            <MapDisplay
              :latitude="user?.latitude"
              :longitude="user?.longitude"
              :readonly="true"
              height="500px"
              @error="handleMapError"
            />
          </v-card-text>
        </v-card>
      </v-dialog>

      <!-- Confirmation Dialog -->
      <v-dialog v-model="confirmDialog" max-width="400" class="confirm-dialog">
        <v-card>
          <v-card-title class="text-h6 font-weight-bold">
            تأكيد الحذف
          </v-card-title>
          <v-card-text>
            هل أنت متأكد من رغبتك في حذف المراقب
            <strong>{{ user.firstname }} {{ user.lastname }}</strong>
            من الفريق؟
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn
              color="grey-darken-1"
              variant="text"
              @click="confirmDialog = false"
            >
              إلغاء
            </v-btn>
            <v-btn
              color="error"
              :loading="deleteLoading"
              @click="confirmDelete"
            >
              حذف
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-card-text>
  </v-card>
</template>

<style lang="scss" scoped>
.user-card {
  background-color: white;
  border: 1px solid rgba(182, 146, 101, 0.12);
  transition: all 0.3s ease;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.04) !important;

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
  }
}

.user-info {
  background-color: #f9f9f9;
  border-radius: 8px;
  padding: 1rem;
}

.cursor-pointer {
  cursor: pointer;
}

.icon-container {
  cursor: pointer;
  background: transparent;
  border: none;
  padding: 5px;
  transition: all 0.2s ease;
  border-radius: 4px;

  &:hover {
    background-color: rgba(182, 146, 101, 0.1);
  }
}

.confirm-dialog {
  :deep(.v-card) {
    border-radius: 8px;
  }
}
</style>
