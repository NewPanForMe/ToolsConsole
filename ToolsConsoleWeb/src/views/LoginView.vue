<template>
  <div class="login-page">
    <main class="login-shell">
      <section class="brand-panel">
        <div class="brand-mark">TC</div>
        <div class="brand-copy">
          <span class="eyebrow">ToolsConsole</span>
          <h1>数据库链接字串维护平台</h1>
          <p>集中维护连接配置、控制账号权限，并以加密方式分发连接串。</p>
        </div>
        <div class="feature-list" aria-label="平台能力">
          <span>JWT 鉴权</span>
          <span>AES 加密落库</span>
          <span>匿名密文取串</span>
        </div>
      </section>

      <t-card class="login-card" :bordered="false">
        <div class="login-title">
          <span class="brand">欢迎回来</span>
          <span class="subtitle">登录后进入连接配置管理台</span>
        </div>

        <t-form ref="formRef" class="login-form" :data="form" label-width="0" @submit="onSubmit">
          <t-form-item name="userName">
            <t-input
              v-model="form.userName"
              size="large"
              placeholder="请输入账号"
              clearable
              autocomplete="username"
            >
              <template #prefix-icon>
                <user-icon />
              </template>
            </t-input>
          </t-form-item>
          <t-form-item name="password">
            <t-input
              v-model="form.password"
              size="large"
              type="password"
              placeholder="请输入密码"
              autocomplete="current-password"
              @enter="onSubmit"
            >
              <template #prefix-icon>
                <lock-on-icon />
              </template>
            </t-input>
          </t-form-item>
          <t-form-item>
            <t-button theme="primary" type="submit" block :loading="loading" size="large">
              登 录
            </t-button>
          </t-form-item>
        </t-form>

        <div class="login-tip">
          <span>默认管理员</span>
          <b>admin / 123</b>
        </div>
      </t-card>
    </main>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { MessagePlugin } from 'tdesign-vue-next';
import { LockOnIcon, UserIcon } from 'tdesign-icons-vue-next';
import { doLogin, isLoggedIn } from '@/stores/auth';

const router = useRouter();
const route = useRoute();
const formRef = ref();
const loading = ref(false);
const form = reactive({ userName: '', password: '' });

const onSubmit = async () => {
  if (!form.userName.trim()) {
    MessagePlugin.warning('请输入账号');
    return;
  }
  if (!form.password) {
    MessagePlugin.warning('请输入密码');
    return;
  }

  loading.value = true;
  try {
    await doLogin(form.userName.trim(), form.password);
    MessagePlugin.success('登录成功');
    const redirect = (route.query.redirect as string) || '/dbconn';
    router.replace(redirect.startsWith('/') ? redirect : '/dbconn');
  } catch (error) {
    MessagePlugin.error((error as Error).message || '登录失败');
  } finally {
    loading.value = false;
  }
};

// 已登录直接进入主页
if (isLoggedIn()) {
  router.replace('/dbconn');
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 24px;
  overflow: hidden;
  background:
    linear-gradient(rgba(255, 255, 255, 0.76), rgba(255, 255, 255, 0.76)),
    linear-gradient(90deg, rgba(0, 82, 217, 0.08) 1px, transparent 1px),
    linear-gradient(0deg, rgba(0, 82, 217, 0.08) 1px, transparent 1px),
    linear-gradient(135deg, #f4f8ff 0%, #edf7f3 54%, #fff7e6 100%);
  background-size: auto, 32px 32px, 32px 32px, auto;
}

.login-shell {
  width: min(980px, 100%);
  min-height: 560px;
  display: grid;
  grid-template-columns: minmax(0, 1fr) 420px;
  align-items: stretch;
  border: 1px solid rgba(0, 82, 217, 0.12);
  border-radius: 18px;
  overflow: hidden;
  background: rgba(255, 255, 255, 0.72);
  box-shadow: 0 28px 80px rgba(21, 43, 77, 0.16);
  backdrop-filter: blur(16px);
}

.brand-panel {
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 56px;
  color: #10213d;
  background:
    linear-gradient(135deg, rgba(0, 82, 217, 0.96), rgba(0, 168, 112, 0.88)),
    linear-gradient(90deg, rgba(255, 255, 255, 0.16) 1px, transparent 1px),
    linear-gradient(0deg, rgba(255, 255, 255, 0.12) 1px, transparent 1px);
  background-size: auto, 40px 40px, 40px 40px;
}

.brand-panel::after {
  content: '';
  position: absolute;
  right: -64px;
  bottom: -86px;
  width: 280px;
  height: 280px;
  border: 42px solid rgba(255, 255, 255, 0.16);
  border-radius: 50%;
}

.brand-mark {
  position: relative;
  z-index: 1;
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 14px;
  color: #0052d9;
  font-size: 22px;
  font-weight: 800;
  background: #ffffff;
  box-shadow: 0 12px 30px rgba(16, 33, 61, 0.18);
}

.brand-copy {
  position: relative;
  z-index: 1;
  max-width: 430px;
  margin-top: 64px;
}

.eyebrow {
  display: inline-flex;
  align-items: center;
  height: 28px;
  padding: 0 12px;
  border-radius: 999px;
  color: #ffffff;
  font-size: 13px;
  font-weight: 600;
  background: rgba(255, 255, 255, 0.18);
}

.brand-copy h1 {
  margin: 22px 0 16px;
  color: #ffffff;
  font-size: 38px;
  line-height: 1.18;
  font-weight: 800;
  letter-spacing: 0;
}

.brand-copy p {
  margin: 0;
  color: rgba(255, 255, 255, 0.86);
  font-size: 16px;
  line-height: 1.8;
}

.feature-list {
  position: relative;
  z-index: 1;
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 48px;
}

.feature-list span {
  height: 32px;
  display: inline-flex;
  align-items: center;
  padding: 0 12px;
  border: 1px solid rgba(255, 255, 255, 0.28);
  border-radius: 999px;
  color: #ffffff;
  font-size: 13px;
  background: rgba(255, 255, 255, 0.12);
}

.login-card {
  display: flex;
  align-items: center;
  border-radius: 0;
  box-shadow: none;
  background: rgba(255, 255, 255, 0.94);
}

.login-card :deep(.t-card__body) {
  width: 100%;
  padding: 56px 44px;
}

.login-title {
  display: flex;
  flex-direction: column;
  margin-bottom: 30px;
}

.brand {
  font-size: 28px;
  line-height: 1.2;
  font-weight: 700;
  color: #10213d;
}

.subtitle {
  font-size: 14px;
  color: #6b7280;
  margin-top: 8px;
}

.login-form :deep(.t-form__item) {
  margin-bottom: 20px;
}

.login-form :deep(.t-input) {
  border-radius: 8px;
}

.login-form :deep(.t-button) {
  height: 44px;
  border-radius: 8px;
  font-weight: 600;
  box-shadow: 0 10px 22px rgba(0, 82, 217, 0.22);
}

.login-tip {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-top: 10px;
  padding: 12px 14px;
  border-radius: 8px;
  font-size: 13px;
  color: #6b7280;
  background: #f6f9fe;
}

.login-tip b {
  color: #10213d;
  font-weight: 700;
}

@media (max-width: 860px) {
  .login-page {
    padding: 24px 16px;
  }

  .login-shell {
    min-height: auto;
    grid-template-columns: 1fr;
  }

  .brand-panel {
    min-height: 260px;
    padding: 32px;
  }

  .brand-copy {
    margin-top: 40px;
  }

  .brand-copy h1 {
    font-size: 30px;
  }

  .login-card :deep(.t-card__body) {
    padding: 34px 28px;
  }
}

@media (max-width: 480px) {
  .brand-panel {
    padding: 28px 22px;
  }

  .brand-copy h1 {
    font-size: 26px;
  }

  .feature-list {
    gap: 8px;
  }

  .login-card :deep(.t-card__body) {
    padding: 30px 22px;
  }

  .login-tip {
    align-items: flex-start;
    flex-direction: column;
  }
}
</style>
