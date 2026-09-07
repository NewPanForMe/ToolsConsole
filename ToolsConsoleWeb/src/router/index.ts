import { createRouter, createWebHashHistory } from 'vue-router';
import { isLoggedIn } from '@/stores/auth';

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { public: true, title: '登录' },
    },
    {
      path: '/',
      name: 'layout',
      component: () => import('@/views/LayoutView.vue'),
      redirect: '/dbconn',
      children: [
        {
          path: 'dbconn',
          name: 'dbconn',
          component: () => import('@/views/DbConnView.vue'),
          meta: { title: '数据库链接字串维护' },
        },
        {
          path: 'user',
          name: 'user',
          component: () => import('@/views/UserView.vue'),
          meta: { title: '用户管理' },
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/dbconn',
    },
  ],
});

router.beforeEach((to) => {
  const logged = isLoggedIn();
  if (!to.meta.public && !logged) {
    return { path: '/login', query: { redirect: to.fullPath } };
  }
  if (to.path === '/login' && logged) {
    return { path: '/dbconn' };
  }
  return true;
});

router.afterEach((to) => {
  const title = (to.meta.title as string | undefined) ?? 'ToolsConsole';
  document.title = `${title} - ToolsConsole`;
});

export default router;
