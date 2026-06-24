<template>
  <AppHeader />
  <v-container class="landing-page pa-0 overflow-hidden" fluid>
    <!-- Hero Section -->
    <section id="hero" class="hero-section bg-white d-flex align-center">
      <div class="hero-overlay" />
      <div class="img-overlay">
        <div class="pattern" />
        <!-- <img src="/img/Path_29.png" alt="" /> -->
      </div>
      <!-- <video autoplay muted loop playsinline class="hero-video">
        <source src="@/assets/video/____اركاب عرفة.mp4" type="video/mp4" />
      </video> -->
      <div class="hero-content">
        <v-container class="position-relative">
          <v-row align="center" justify="center">
            <v-col class="text-center" cols="12" md="8" style="max-width: 590px">
              <h1 class="font-weight-bold text-white mb-4 title" data-aos="fade-up" data-aos-duration="800">
                منصة جدة سوبردوم لحجٍ منظم وطمأنينة رقمية
              </h1>
              <p
                class="text-body text-white mt-10 px-8 mx-auto subtitle"
                data-aos="fade-up"
                data-aos-delay="200"
                data-aos-duration="800"
              >
                منصة رقمية تسهّل على الحاج كل خطوة، من لحظة الوصول وحتى صعود القافلة.
              </p>
              <div class="d-flex justify-center mt-8">
                <router-link to="#">
                  <span
                    class="d-block font-weight-medium bg-white px-3 py-2 text-white rounded-lg mt-10"
                    data-aos="fade-up"
                    data-aos-delay="400"
                    data-aos-duration="800"
                  >
                    <qr-app />
                  </span>
                </router-link>
              </div>
            </v-col>
          </v-row>
        </v-container>
      </div>
    </section>

    <!-- Why Join Section -->
    <section id="services" class="bg-white py-8">
      <v-container>
        <v-row>
          <v-col cols="12" md="8">
            <h3 class="text-secondary mb-3" data-aos="fade-up">الخدمات المقدمة</h3>
            <p class="font-weight-bold text-h5" data-aos="fade-up" data-aos-delay="100">مزايا ترافقك في كل خطوة</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="mb-0" cols="12" lg="4" sm="6">
            <v-card class="benefits-card h-100 pa-6" data-aos="fade-up" data-aos-delay="200" flat>
              <div class="d-flex mb-6">
                <Database />
              </div>
              <div class="text-h6 font-weight-bold mb-4">بياناتك في مكان واحد</div>
              <p class="text-grey">اعرف كل تفاصيل رحلتك مثل: وقت التفويج، رقم البوابة، والمسار المخصص لك.</p>
            </v-card>
          </v-col>

          <v-col class="mb-0" cols="12" lg="4" sm="6">
            <v-card class="benefits-card h-100 pa-6" data-aos="fade-up" data-aos-delay="300" flat>
              <div class="d-flex mb-6">
                <Location />
              </div>
              <div class="text-h6 font-weight-bold mb-4">تحديث لحظي لحالتك</div>
              <p class="text-grey">تابع حالتك خطوة بخطوة من لحظة الوصول حتى صعود الحافلة.</p>
            </v-card>
          </v-col>

          <v-col class="mb-0" cols="12" lg="4" sm="6">
            <v-card class="benefits-card h-100 pa-6" data-aos="fade-up" data-aos-delay="400" flat>
              <div class="d-flex mb-6">
                <Notification />
              </div>

              <div class="text-h6 font-weight-bold mb-4">إشعارات تنبهك في الوقت المناسب</div>
              <p class="text-grey">إشعارات تنبهك في الوقت المناسب تنبيهات فورية بكل ما يخص حركتك داخل السوبر دوم.</p>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- features -->
    <section id="features" class="py-10 bg-grey-lighten-5">
      <v-container>
        <v-row class="align-center">
          <v-col class="mb-0 pl-md-24" cols="12" sm="6">
            <div>
              <h3 class="text-secondary mb-4" data-aos="fade-right">خدمات لتجربة مثالية</h3>
              <h4 class="font-weight-bold text-h4 mb-4" data-aos="fade-right" data-aos-delay="50">
                حالتك تُحدّث في لحظات
              </h4>
              <p class="font-weight-medium text-sm text-grey" data-aos="fade-right" data-aos-delay="100">
                بمجرد مسح رمز الـ QR، يتم تحديث حالتك تلقائيًا في كل مرحلة داخل السوبر دوم — بسهولة وسرعة.
              </p>
            </div>
          </v-col>
          <v-col class="mb-0" cols="12" sm="6">
            <img
              alt="features"
              class="rounded-xl"
              data-aos="fade-left"
              data-aos-delay="200"
              src="/img/features_1.jpg"
              style="width: 100%; max-width: 628px"
            />
          </v-col>
        </v-row>
      </v-container>
      <div class="py-6" />
      <v-container>
        <v-row class="align-center flex-row-reverse">
          <v-col class="d-flex pr-md-20" cols="12" sm="6">
            <div>
              <h3 class="text-secondary mb-4" data-aos="fade-left">خدمات لتجربة مثالية</h3>
              <h4 class="font-weight-bold text-h4 mb-4" data-aos="fade-left" data-aos-delay="50">
                خريطة ترافقك في كل خطوة
              </h4>
              <p class="font-weight-medium text-sm text-grey" data-aos="fade-left" data-aos-delay="100">
                اعرف طريقك داخل السوبر دوم من أول لحظة. الخريطة التفاعلية توضّح لك مكانك وتدلّك على المسار الصحيح.
              </p>
            </div>
          </v-col>
          <v-col class="mb-0" cols="12" sm="6">
            <img
              alt="features"
              class="rounded-xl"
              data-aos="fade-right"
              data-aos-delay="200"
              src="/img/features_2.jpg"
              style="width: 100%; max-width: 628px"
            />
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- FAQ Section -->
    <section id="faq" class="faq_section py-16 bg-white">
      <v-container class="bg-grey-lighten-5 py-6 rounded-xl px-md-8">
        <v-row>
          <v-col cols="12" md="8">
            <h2 class="text-h5 font-weight-bold mb-2" data-aos="fade-up">الأسئلة الشائعة</h2>
          </v-col>
        </v-row>
        <v-row justify="center">
          <v-col cols="12">
            <v-expansion-panels variant="accordion">
              <template #default>
                <div
                  v-for="(faq, index) in faqs"
                  :key="index"
                  class="w-100 d-block"
                  data-aos="slide-up"
                  :data-aos-delay="Number(index) * 50"
                >
                  <v-expansion-panel class="mb-2">
                    <v-expansion-panel-title class="text-h6 font-weight-bold">
                      <span class="text-grey-600 rounded-lg bg-grey-100 icon-num ml-2">
                        {{ Number(index) + 1 }}
                      </span>
                      <p class="faq-question mb-0">{{ faq.question }}</p>
                      <template #actions="{ expanded }">
                        <span class="icon-num rounded-lg bg-grey-100">
                          <v-icon v-if="expanded" class="icon-faq text-grey-600" size="16"> mdi-minus </v-icon>
                          <v-icon v-else class="icon-faq text-grey-600" size="16"> mdi-plus </v-icon>
                        </span>
                      </template>
                    </v-expansion-panel-title>
                    <v-expansion-panel-text class="text-grey-700">
                      {{ faq.answer }}
                    </v-expansion-panel-text>
                  </v-expansion-panel>
                </div>
              </template>
            </v-expansion-panels>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <section class="page-footer py-10">
      <v-container>
        <v-row class="justify-center">
          <v-col cols="12" md="7" sm="8">
            <div class="d-flex justify-center mb-10" data-aos="fade-up" data-aos-delay="50">
              <Logo data-aos="zoom-in" :height="140" :width="236" />
            </div>
          </v-col>
          <v-col class="text-center" cols="12" style="max-width: 700px">
            <p class="text-white text-sm text-h6 px-8 mx-auto" data-aos="fade-up" data-aos-delay="100">
              نحن لا نقد لك تقنية فحسب ، بل نرافقك فى درب الطمأنينة...
              <br />
              نسأل الله لك حجاً مبروراً وذنباً مغفوراً...
            </p>
          </v-col>
        </v-row>
      </v-container>
    </section>
  </v-container>

  <AppFooter />
</template>

<script setup lang="ts">
import AOS from 'aos'
import 'aos/dist/aos.css'

import QrApp from '@/components/icons/qr-app.vue'
import Database from '@/components/icons/database.vue'
import Location from '@/components/icons/location.vue'
import Notification from '@/components/icons/notification.vue'
import Logo from '@/components/icons/logo.vue'
import AppHeader from '@/components/AppHeader.vue'
import AppFooter from '@/components/AppFooter.vue'

onMounted(() => {
  setTimeout(() => {
    AOS.init({
      duration: 500,
      easing: 'ease-in-out',
      once: true,
      offset: 100,
    })
  }, 100)
})

const faqs = [
  {
    question: 'كيف أدخل إلى التطبيق',
    answer: 'أدخل رقم الهوية الرمز السرى PIN لعرض بياناتك ومتابعة حالتك',
  },
  {
    question: 'ما هي الخدمات المتوفرة داخل التطبيق؟',
    answer:
      'المتابعة الميدانية لقياس حالة الامتثال شركات تقديم الخدمة في مواقع تواجد ضيوف الرحمن في مكة المكرمة و المشاعر المقدسة',
  },
  {
    question: 'متى موعد تحركي؟',
    answer: 'تختلف فترات العمل حسب الوردية والمسمى الوظيفي الخاص بكل موظف',
  },
  {
    question: 'متى يتم تحديث حالتي داخل السوبر دوم؟',
    answer: 'تختلف فترات العمل حسب الوردية والمسمى الوظيفي الخاص بكل موظف',
  },
  {
    question: 'هل التطبيق يدعم لغات غير العربية؟',
    answer: 'تختلف فترات العمل حسب الوردية والمسمى الوظيفي الخاص بكل موظف',
  },
]
</script>

<style scoped lang="scss">
.landing-page {
  direction: rtl;

  .v-container {
    max-width: 1280px !important;
  }

  .page-footer {
    background: linear-gradient(180deg, #0d114e 0%, #1e27b4 100%);

    .footerparagraph {
      word-break: keep-all;
      white-space: wrap;
      hyphens: inherit;
      text-align: center;
      /* Ensures proper Arabic alignment */
      direction: rtl;
    }
  }

  section {
    scroll-margin-top: 80px;
  }

  @media (min-width: 960px) {
    .pr-md-20 {
      padding-right: 4.4rem;
    }

    .pl-md-24 {
      padding-left: 8rem;
    }
  }

  @media (max-width: 768px) {
    .title {
      font-size: 40px !important;
      line-height: 65px !important;
    }
  }

  .hero-section {
    // background: url('@/assets/images/header.webp');
    // background-size: cover;
    // background-position: center;
    position: relative;
    color: white;
    min-height: 100vh;
    position: relative;
    padding-top: 0px;

    .hero-overlay {
      position: absolute;
      width: 2327.67px;
      height: 2327.67px;
      left: 50%;
      transform: translateX(-50%) rotate(90deg);
      bottom: 0px;
      background: linear-gradient(180deg, #0d114e 0%, #1e27b4 100%);
      border-radius: 23580.9px;
      flex: none;
      order: 1;
      flex-grow: 0;
      z-index: 1;
    }

    .img-overlay {
      position: absolute;
      top: 0px;
      left: 0px;
      width: 100%;
      height: 100%;
      z-index: 2;
      overflow: hidden;

      .pattern {
        width: 100%;
        height: 100%;
        background: url('/img/Path_29.png') no-repeat center center;
        background-size: cover;
        background-repeat: no-repeat;
        background-position: center 70px;
        background-attachment: fixed;
      }

      img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }
    }

    .hero-content {
      position: relative;
      width: 100%;
      z-index: 100;
    }

    .v-container {
      position: relative;
      z-index: 1;

      .title {
        font-size: 60px;
        line-height: 77px;
      }

      .subtitle {
        max-width: 500px;
        font-size: 24px;
        line-height: 36px;
      }
    }

    &::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      // background: rgba(0, 0, 0, 0.4);
      z-index: 1;
    }
  }

  // .why_join {
  //   position: relative;
  //   text-align: center;
  //   color: white;

  //   &::before {
  //     content: "";
  //     position: absolute;
  //     top: 0;
  //     left: 0;
  //     right: 0;
  //     bottom: 0;
  //     background: url("@/assets/images/banar.webp");
  //     background-size: cover;
  //     background-position: center;
  //     background-repeat: no-repeat;
  //     filter: brightness(0.8);
  //     z-index: 1;
  //   }

  //   .v-container {
  //     // padding: 0;
  //     position: relative;
  //     z-index: 2;

  //     p {
  //       font-size: 24px;
  //     }
  //   }
  // }

  .question {
    font-size: 24px;
  }

  // .city-skyline-section {
  //   background: url("@/assets/images/city-skyline.svg") no-repeat center bottom;
  //   min-height: 200px;
  //   margin-top: 2rem;
  // }
  .v-expansion-panel {
    border-radius: 12px !important;

    &::after {
      display: none;
    }
  }

  .landing-page[data-v-ce1f75b8] .v-expansion-panels .v-expansion-panel {
    border-radius: 12px !important;
  }

  :deep(.v-expansion-panels) {
    .v-expansion-panel {
      background-color: transparent;
      overflow: hidden;

      &-title {
        font-weight: 500;
        padding: 1rem 1.5rem;
        background-color: white;

        &:hover {
          .v-expansion-panel-title__overlay {
            background-color: #000;
            opacity: 0.04;
          }
        }
      }

      &-text {
        .v-expansion-panel-text__wrapper {
          padding: 1rem 1.8rem !important;
        }
      }
    }
  }

  .v-btn {
    text-transform: none;
    letter-spacing: 0;
  }

  .requirements-tabs {
    :deep(.v-tabs) {
      .v-tab {
        text-transform: none;
        letter-spacing: 0;
        font-size: 1rem;
        padding: 0 2rem;
        min-width: 200px;

        .v-icon {
          margin-left: 8px;
        }
      }
    }

    .requirements-list {
      background: transparent;
      padding: 1rem;

      .v-list-item {
        background: white;
        border-radius: 8px;
        border: 1px solid rgba(0, 0, 0, 0.1);
        padding: 1rem;
        margin-bottom: 1rem;

        .requirement-number {
          width: 32px;
          height: 32px;
          border-radius: 50%;
          background: rgb(var(--v-theme-primary));
          color: white;
          display: flex;
          align-items: center;
          justify-content: center;
          font-weight: 500;
          margin-left: 1rem;
        }

        &-title {
          font-size: 1rem;
          line-height: 1.5;
        }
      }
    }
  }

  .benefits-card {
    transition: all 0.3s ease;
    background-color: rgb(250, 250, 250);
    // border: 1px solid rgba(0, 0, 0, 0.05);
    // text-align: center;
    border-radius: 16px !important;

    &:hover {
      transform: translateY(-5px);
      box-shadow: 0 4px 25px 0 rgba(0, 0, 0, 0.1);
    }

    .benefits-icon {
      // background: rgba(var(--v-theme-primary), 0.1);
      padding: 16px;
      border-radius: 50%;
    }
  }
}

.v-expansion-panel-text {
  padding: 0 !important;
}

.landing-page .requirements-tabs[data-v-ce1f75b8] .v-tabs .v-tab {
  border-radius: 0 !important;
}

.v-slide-group__content {
  .v-tab {
    border-radius: 0 !important;

    // font-weight: 900 !important;
    span {
      color: #75797c !important;
    }

    svg {
      margin-left: 8px;

      // fill: gray !important;
      path {
        fill: gray !important;
      }
    }
  }

  .v-tab-item--selected {
    span {
      color: #000 !important;
      font-weight: 900 !important;
    }

    svg {
      // fill: gray !important;
      path {
        fill: rgb(var(--v-theme-primary)) !important;
      }
    }
  }
}

.faq-question {
  font-size: 20px;

  @media (max-width: 768px) {
    font-size: 16px;
  }
}

.icon-num {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  font-size: 16px;
}

.gallery-card {
  border-radius: 12px;
  overflow: hidden;

  .gallery-wrapper {
    .gallery-carousel {
      :deep(.v-carousel__controls) {
        display: none;
      }

      :deep(.v-window__container) {
        cursor: grab;

        &:active {
          cursor: grabbing;
        }
      }
    }
  }

  :deep(.v-img) {
    transition: transform 0.3s ease;

    &:hover {
      transform: scale(1.05);
    }
  }
}

.social-img {
  position: relative;
  width: 433px;
  height: 422px;
  display: flex;
  align-items: center;
  justify-content: center;

  img {
    height: 422px;
    width: 433px;
    z-index: 1;
  }

  // margin: 2rem auto;

  // &::after {
  //   content: "";
  //   position: absolute;
  //   top: 35px;
  //   right: 53px;
  //   background: url("@/assets/images/frame_img.webp");
  //   background-size: contain;
  //   background-position: center;
  //   background-repeat: no-repeat;
  //   z-index: 0;
  //   pointer-events: none;
  //   width: 100%;
  //   height: 100%;
  // }

  :deep(.v-img) {
    position: relative;
    z-index: 1;
    width: 450px;
    height: 450px;
    border-radius: 16px;
    overflow: hidden;
    box-shadow: 0 4px 25px rgba(0, 0, 0, 0.1);
  }
}

.imtithal-carousel {
  :deep(.v-carousel__controls) {
    margin-bottom: 24px;

    .v-btn--icon.v-btn--density-default {
      width: 20px !important;
      height: 3px !important;
      margin: 0 4px;
      border-radius: 0;
      background: rgba(var(--v-theme-secondary), 0.3);
      transform-origin: center;
      transition: all 0.3s ease;

      .v-btn__content {
        display: none;
      }

      &.v-btn--active {
        background: rgb(var(--v-theme-secondary));
        width: 30px !important;
      }

      &:hover {
        background: rgb(var(--v-theme-secondary));
      }
    }

    .v-btn {
      min-width: unset;
      padding: 0;
      margin: 0 4px;
      opacity: 1;

      &::before {
        display: none;
      }

      &--active {
        transform: none;
      }
    }
  }

  :deep(.v-carousel__progress) {
    display: none;
  }
}

.gallery-carousel {
  :deep(.v-window__container) {
    cursor: grab;

    &:active {
      cursor: grabbing;
    }
  }

  :deep(.v-window-item) {
    transition: transform 0.3s ease-out;

    .v-row {
      margin: 0 -8px;

      .v-col {
        padding: 0 8px;
      }
    }
  }

  &.dragging {
    :deep(.v-window-item) {
      transition: none;
    }
  }

  :deep(.v-img) {
    transition: transform 0.3s ease;
    height: 100%;
    width: 100%;

    &:hover {
      transform: scale(1.05);
    }
  }

  :deep(.v-carousel__controls) {
    .v-btn {
      margin: 0 4px;

      &.v-btn--active {
        transform: scale(1.2);
      }
    }
  }
}

.gallery-wrapper {
  .v-row {
    margin: 0;
  }
}
</style>
