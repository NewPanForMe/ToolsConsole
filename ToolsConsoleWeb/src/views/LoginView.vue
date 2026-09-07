<template>
  <div class="login-page">
    <t-card class="login-card" :bordered="false">
      <template #title>
        <div class="login-title">
          <span class="brand">ToolsConsole</span>
          <span class="subtitle">数据库链接字串维护平台</span>
        </div>
      </template>

      <t-form ref="formRef" :data="form" @submit="onSubmit">
        <t-form-item label="账号" name="userName">
          <t-input v-model="form.userName" placeholder="请输入账号" clearable autocomplete="username" />
        </t-form-item>
        <t-form-item label="密码" name="password">
          <t-input
            v-model="form.password"
            type="password"
            placeholder="请输入密码"
            autocomplete="current-password"
            @enter="onSubmit"
          />
        </t-form-item>
        <t-form-item>
          <t-button theme="primary" type="submit" block :loading="loading" size="large">
            登 录
          </t-button>
        </t-form-item>
      </t-form>

      <div class="login-tip">默认管理员：admin / 123</div>
    </t-card>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { MessagePlugin } from 'tdesign-vue-next';
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
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #0052d9 0%, #7a9efd 55%, #b0c6ff 100%);
}

.login-card {
  width: 380px;
  border-radius: 10px;
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.16);
}

.login-title {
  display: flex;
  flex-direction: column;
}

.brand {
  font-size: 22px;
  font-weight: 700;
  color: #0052d9;
}

.subtitle {
  font-size: 13px;
  color: #888;
  margin-top: 4px;
}

.login-tip {
  margin-top: 8px;
  font-size: 12px;
  color: #999;
  text-align: center;
}
</style>
