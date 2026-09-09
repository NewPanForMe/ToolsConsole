<template>
  <!-- 悬浮卡片外壳：四周留白，整卡圆角不贴屏幕边缘 -->
  <div class="app-shell">
    <t-layout class="layout">
      <!-- 整宽顶栏 -->
      <t-header class="header">
        <div class="header-left">
          <span class="brand">ToolsConsole</span>
          <span class="divider" />
          <span class="page-title">{{ pageTitle }}</span>
        </div>
        <div class="header-right">
          <span class="user-name">{{ authState.displayName || authState.userName }}</span>
          <t-button variant="outline" theme="default" size="small" @click="onLogout">退出登录</t-button>
        </div>
      </t-header>

      <!-- 主体：左侧菜单 + 内容区 -->
      <t-layout class="body">
        <t-aside class="aside" width="250px">
          <t-menu class="side-menu" :value="activePath" theme="light" @change="onMenuChange">
            <t-menu-item value="/dbconn">
              <template #icon>
                <t-icon name="server" />
              </template>
              数据库链接字串维护
            </t-menu-item>
            <t-menu-item value="/user">
              <template #icon>
                <t-icon name="user" />
              </template>
              用户管理
            </t-menu-item>
            <t-menu-item value="/system-config">
              <template #icon>
                <t-icon name="setting" />
              </template>
              系统配置
            </t-menu-item>
            <t-menu-item value="/site-config">
              <template #icon>
                <t-icon name="view-module" />
              </template>
              站点配置
            </t-menu-item>
          </t-menu>
        </t-aside>

        <t-layout class="main">
          <t-content class="content">
            <router-view />
          </t-content>
        </t-layout>
      </t-layout>
    </t-layout>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { authState, logout } from '@/stores/auth';

const route = useRoute();
const router = useRouter();

const menuPaths = ['/dbconn', '/user', '/system-config', '/site-config'];
const activePath = computed(() => (menuPaths.includes(route.path) ? route.path : ''));
const pageTitle = computed(() => (route.meta.title as string | undefined) ?? 'ToolsConsole');

const onMenuChange = (value: string | number) => {
  router.push(String(value));
};

const onLogout = () => {
  logout();
  router.replace('/login');
};
</script>

<style scoped>
/* ---------- 外壳：四周留白，不贴屏幕边缘 ---------- */
.app-shell {
  height: 100vh;
  width: 100%;
  padding: 12px;
  box-sizing: border-box;
  background: #e8ecf3;
  display: flex;
  flex-direction: column;
}

/* 悬浮卡片本体：圆角 + 阴影，内部各区域精确对齐 */
.layout {
  flex: 1 1 auto;
  min-height: 0;
  display: flex;
  flex-direction: column;
  background: #fff;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 18px rgba(15, 40, 90, 0.1);
}

/* ---------- 顶栏 ---------- */
.header {
  flex: 0 0 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 0 20px;
  background: #fff;
  border-bottom: 1px solid #ececec;
  box-sizing: border-box;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 0;
}

.brand {
  font-size: 16px;
  font-weight: 700;
  color: #0052d9;
  white-space: nowrap;
}

.divider {
  width: 1px;
  height: 16px;
  background: #e0e0e0;
  flex-shrink: 0;
}

.page-title {
  font-size: 14px;
  color: #555;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.user-name {
  color: #333;
  white-space: nowrap;
}

/* ---------- 主体 ---------- */
.body {
  flex: 1 1 auto;
  min-height: 0;
  display: flex;
  flex-direction: row;
}

.aside {
  flex: 0 0 250px;
  width: 250px;
  background: #fff;
  border-right: 1px solid #ececec;
  display: flex;
  box-sizing: border-box;
  /* 上下左右均留内边距，重点保证菜单右侧不贴边 */
  padding: 12px 12px 12px 10px;
  overflow: hidden;
}

.side-menu {
  flex: 1 1 auto;
  min-width: 0;
  border: none;
  border-radius: 10px;
  overflow: hidden;
}

.main {
  flex: 1 1 auto;
  min-width: 0;
  display: flex;
  flex-direction: column;
  background: #f5f7fb;
}

.content {
  flex: 1 1 auto;
  min-height: 0;
  padding: 16px;
  overflow: auto;
  box-sizing: border-box;
}
</style>
