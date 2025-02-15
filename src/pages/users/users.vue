<script setup>
import { useAppStore } from "@/stores/app";
import UsersTable from "./components/UsersTable.vue";

const appStore = useAppStore();
const isAddingRoleDialog = ref(false);
const isAddingDepartmentDialog = ref(false);
const isAddingEmployeeDialog = ref(false);
const tab = ref("general");

onMounted(() => {
  appStore.updateAppPageTitle("المستخدمين");
});
</script>

<template>
  <div>
    <UsersTable />
  </div>
</template>

<style lang="scss">
.v-card-item {
  padding: 10px !important;
  .icon-wrapper {
    margin: 0 !important;
  }
}
.col-gray {
  background-color: #f5f5f5 !important;
  padding: 20px !important;
  border-radius: 12px;
}
.border-dark-yellow-2 {
  border-radius: 12px;
  border: 1px solid #f79009;
}
.border-dark-yellow-2 {
  border-radius: 12px;
  border: 1px solid #f79009;
}
.border-dark-green-2 {
  border-radius: 12px;
  border: 1px solid #079455;
}
.three-item-grid {
  flex-wrap: wrap;
  &__item {
    width: calc((100% - 4px) / 3);
  }
}

.checkbox-container {
  display: flex;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  overflow: hidden;

  .checkBoxDay {
    position: relative;
    padding: 8px 16px;
    border-right: 1px solid #e5e7eb;
    min-width: 80px;
    height: 40px;
    display: flex;
    justify-content: center;
    flex-shrink: 0;
    &:first-child {
      border-right: none;
    }

    .v-selection-control__wrapper {
      position: absolute;
    }

    input {
      position: relative;
      left: -34px;
      top: 0;
      width: 100%;
      height: 100%;
      opacity: 0;
    }

    svg {
      display: none;
    }

    .v-selection-control__input {
      &::before {
        display: none;
      }
    }

    &:has(input:checked) {
      background-color: #00758d;
      color: white;
    }

    .v-label--clickable {
      display: flex;
      justify-content: center;
      align-items: center;
      width: 100%;
      height: 100%;
    }
  }
}
</style>
