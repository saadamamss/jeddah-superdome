<template>
  <div class="d-flex align-center text-center justify-space-between">
    <div class="d-flex align-center text-center">
      <v-avatar size="40" class="mb-2">
        <v-img
          :src="user.avatar_url || '/imgs/person.svg'"
          alt="Supervisor"
          width="40"
          height="40"
        ></v-img>
      </v-avatar>
      <div class="d-flex flex-column justify-space-between align-center">
        <div class="d-flex align-center">
          <h3 class="text-h6 mb-1">{{ user.fullname }}</h3>
        </div>

        <div class="d-flex flex-column align-center">
          <div
            class="text-body-2 text-medium-emphasis mb-4 d-flex align-center ms-4"
          >
            <!-- {{ user }} -->
            <div class="text-body-1">{{ user.sa_id }}</div>
          </div>
        </div>
      </div>
    </div>
    <div @click="superSupervisorsDt" class="cursor-pointer">
      <svg
        width="40"
        height="40"
        viewBox="0 0 40 40"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M10.667 0.333008H29.333C35.0399 0.333008 39.667 4.96005 39.667 10.667V29.333C39.667 35.0399 35.0399 39.667 29.333 39.667H10.667C4.96005 39.667 0.333008 35.0399 0.333008 29.333V10.667C0.333008 4.96005 4.96005 0.333008 10.667 0.333008Z"
          stroke="#E9EBEC"
          stroke-width="0.666667"
        />
        <path
          opacity="0.4"
          fill-rule="evenodd"
          clip-rule="evenodd"
          d="M26.6673 20.0007C26.6673 19.7903 26.566 19.6482 26.3633 19.364C25.4526 18.0869 23.1268 15.334 20.0007 15.334C16.8745 15.334 14.5487 18.0869 13.638 19.364C13.4353 19.6482 13.334 19.7903 13.334 20.0007C13.334 20.211 13.4353 20.3531 13.638 20.6373C14.5487 21.9144 16.8745 24.6673 20.0007 24.6673C23.1268 24.6673 25.4526 21.9144 26.3633 20.6373C26.566 20.3531 26.6673 20.211 26.6673 20.0007ZM20.0007 18.0007C21.1052 18.0007 22.0007 18.8961 22.0007 20.0007C22.0007 21.1052 21.1052 22.0007 20.0007 22.0007C18.8961 22.0007 18.0007 21.1052 18.0007 20.0007C18.0007 18.8961 18.8961 18.0007 20.0007 18.0007Z"
          fill="#B69265"
        />
        <path
          fill-rule="evenodd"
          clip-rule="evenodd"
          d="M20.0007 22.5007C18.6199 22.5007 17.5007 21.3814 17.5007 20.0007C17.5007 18.6199 18.6199 17.5007 20.0007 17.5007C21.3814 17.5007 22.5007 18.6199 22.5007 20.0007C22.5007 21.3814 21.3814 22.5007 20.0007 22.5007ZM18.5007 20.0007C18.5007 20.8291 19.1722 21.5007 20.0007 21.5007C20.8291 21.5007 21.5007 20.8291 21.5007 20.0007C21.5007 19.1722 20.8291 18.5007 20.0007 18.5007C19.1722 18.5007 18.5007 19.1722 18.5007 20.0007Z"
          fill="#B69265"
        />
        <path
          fill-rule="evenodd"
          clip-rule="evenodd"
          d="M20.0007 14.834C18.2869 14.834 16.818 15.5883 15.6809 16.476C14.5427 17.3645 13.7015 18.4138 13.2309 19.0737L13.1954 19.1231C13.0267 19.3579 12.834 19.6258 12.834 20.0007C12.834 20.3755 13.0267 20.6434 13.1954 20.8782L13.2309 20.9276C13.7015 21.5875 14.5427 22.6368 15.6809 23.5253C16.818 24.413 18.2869 25.1673 20.0007 25.1673C21.7144 25.1673 23.1834 24.413 24.3204 23.5253C25.4586 22.6368 26.2998 21.5875 26.7704 20.9276L26.8059 20.8782C26.9746 20.6434 27.1673 20.3755 27.1673 20.0007C27.1673 19.6258 26.9746 19.3579 26.8059 19.1231L26.7704 19.0737C26.2998 18.4138 25.4586 17.3645 24.3204 16.476C23.1833 15.5883 21.7144 14.834 20.0007 14.834ZM14.0451 19.6543C14.4852 19.0371 15.2622 18.0714 16.2962 17.2642C17.3312 16.4562 18.5882 15.834 20.0007 15.834C21.4131 15.834 22.6701 16.4562 23.7051 17.2642C24.7391 18.0714 25.5161 19.0371 25.9562 19.6543C26.0651 19.807 26.1169 19.8815 26.1478 19.9409C26.1674 19.9786 26.1674 19.9889 26.1673 19.9991L26.1673 20.0007L26.1673 20.0022C26.1674 20.0124 26.1674 20.0227 26.1478 20.0604C26.1169 20.1198 26.0651 20.1943 25.9562 20.347C25.5161 20.9642 24.7391 21.9299 23.7051 22.7371C22.6701 23.5451 21.4131 24.1673 20.0007 24.1673C18.5882 24.1673 17.3312 23.5451 16.2962 22.7371C15.2622 21.9299 14.4852 20.9642 14.0451 20.347C13.9362 20.1943 13.8844 20.1198 13.8535 20.0604C13.8339 20.0227 13.8339 20.0124 13.834 20.0022L13.834 20.0007L13.834 19.9991C13.8339 19.9889 13.8339 19.9786 13.8535 19.9409C13.8844 19.8815 13.9362 19.807 14.0451 19.6543Z"
          fill="#B69265"
        />
      </svg>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { hasPermission } from "@core/utils/permissions";
import { useRouter } from "vue-router";

defineOptions({
  name: "ResidenceSupervisorsCard",
});

const router = useRouter();

const props = defineProps({
  user: {
    type: Object,
    required: true,
    default: () => ({
      fullname: "",
      sa_id: "",
      avatar_url: null,
      phone: "",
      email: "",
      gender: "",
      nationality: "",
    }),
  },
  visitCount: {
    type: Number,
    default: 0,
  },
});

const superSupervisorsDt = () => {
  if (props.user && props.user.id) {
    router.push(`/attributions/supervisors_dt/${props.user.id}`);
  }
};

const canEdit = hasPermission("teams.update");
const canDelete = hasPermission("teams.delete");

const getInitials = computed(() => {
  if (!props.user.fullname) return "";
  const nameParts = props.user.fullname.split(" ");
  if (nameParts.length >= 2) {
    return `${nameParts[0].charAt(0)}${nameParts[1].charAt(0)}`;
  }
  return props.user.fullname.charAt(0);
});
</script>

<style lang="scss" scoped>
.v-avatar {
  border: 2px solid rgb(var(--v-theme-warning));
}

.cursor-pointer {
  cursor: pointer;
}
</style>
