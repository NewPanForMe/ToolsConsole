<template>
  <div class="user-page">
    <!-- 工具栏 -->
    <t-card :bordered="false" class="toolbar-card">
      <div class="toolbar">
        <t-input
          v-model="keyword"
          class="keyword-input"
          placeholder="按用户名 / 显示名搜索"
          clearable
          @enter="onSearch"
          @clear="onReset"
        >
          <template #prefix-icon>
            <search-icon />
          </template>
        </t-input>
        <t-button theme="primary" @click="onSearch">查询</t-button>
        <t-button theme="default" @click="onReset">重置</t-button>
        <div class="spacer" />
        <t-button theme="primary" @click="openCreate">
          <template #icon><add-icon /></template>
          新增用户
        </t-button>
      </div>
    </t-card>

    <!-- 列表 -->
    <t-card :bordered="false" class="list-card">
      <t-table
        :data="rows"
        :columns="columns"
        :loading="loading"
        row-key="id"
        table-layout="fixed"
        hover
        size="medium"
      >
        <template #status="{ row }">
          <t-switch
            :value="row.status === 1"
            :disabled="isStatusLocked(row)"
            @change="(val: string | number | boolean) => toggleStatus(row, Boolean(val))"
          />
        </template>
        <template #operation="{ row }">
          <t-space>
            <t-button size="small" variant="text" theme="default" @click="openEdit(row)">编辑</t-button>
            <t-button size="small" variant="text" theme="primary" @click="openReset(row)">重置密码</t-button>
            <t-button
              size="small"
              variant="text"
              theme="danger"
              :disabled="!canDelete(row)"
              @click="openDelete(row)"
            >
              删除
            </t-button>
          </t-space>
        </template>
      </t-table>

      <div class="pagination">
        <t-pagination
          v-model:current="pageIndex"
          v-model:pageSize="pageSize"
          :total="total"
          :page-size-options="[10, 20, 50]"
          show-jumper
          @change="loadData"
        />
      </div>
    </t-card>

    <!-- 新增/编辑用户 对话框 -->
    <t-dialog
      :visible="dialogVisible"
      :header="editingId ? '编辑用户' : '新增用户'"
      :width="520"
      cancel-btn="取消"
      confirm-btn="保存"
      :on-cancel="closeDialog"
      :on-close="closeDialog"
      :on-overlay-click="closeDialog"
      :on-confirm="submitForm"
      :confirm-loading="submitting"
    >
      <t-form :data="form" label-align="right">
        <t-form-item label="用户名" name="userName">
          <t-input
            v-model="form.userName"
            :disabled="editingId !== null"
            placeholder="字母/数字/下划线/中划线/点，2-64 位"
          />
        </t-form-item>
        <t-form-item label="显示名" name="displayName">
          <t-input v-model="form.displayName" placeholder="留空则使用用户名" />
        </t-form-item>
        <t-form-item v-if="editingId === null" label="初始密码" name="password" required-mark>
          <t-input
            v-model="form.password"
            type="password"
            placeholder="3-64 位，保存后自动加密"
            clearable
          />
        </t-form-item>
        <t-form-item v-if="editingId !== null" label="状态" name="status">
          <t-radio-group v-model="form.status" :disabled="isStatusLocked(editRow)">
            <t-radio :value="1">启用</t-radio>
            <t-radio :value="0">禁用</t-radio>
          </t-radio-group>
        </t-form-item>
        <t-form-item v-if="editingId !== null && isStatusLocked(editRow)">
          <span class="form-tip">内置 admin 与当前登录账号不允许被禁用</span>
        </t-form-item>
      </t-form>
    </t-dialog>

    <!-- 重置密码 对话框 -->
    <t-dialog
      :visible="resetVisible"
      header="重置密码"
      :width="440"
      cancel-btn="取消"
      confirm-btn="重置"
      :on-cancel="closeReset"
      :on-close="closeReset"
      :on-overlay-click="closeReset"
      :on-confirm="confirmReset"
      :confirm-loading="resetting"
    >
      <t-form :data="resetForm" label-align="right">
        <t-form-item label="目标用户">
          <t-input :value="resetTarget?.displayName || resetTarget?.userName" readonly />
        </t-form-item>
        <t-form-item label="新密码" name="password" required-mark>
          <t-input v-model="resetForm.password" type="password" placeholder="3-64 位" clearable />
        </t-form-item>
        <t-form-item label="确认密码" name="confirm" required-mark>
          <t-input v-model="resetForm.confirm" type="password" placeholder="再次输入新密码" clearable />
        </t-form-item>
      </t-form>
    </t-dialog>

    <!-- 删除确认 对话框 -->
    <t-dialog
      :visible="deleteVisible"
      header="删除确认"
      cancel-btn="取消"
      confirm-btn="删除"
      theme="warning"
      :on-cancel="closeDelete"
      :on-close="closeDelete"
      :on-overlay-click="closeDelete"
      :on-confirm="confirmDelete"
      :confirm-loading="deleting"
      :width="420"
    >
      <span>
        确定删除用户「<b>{{ deleteTarget?.displayName || deleteTarget?.userName }}</b>」？删除后不可恢复。
      </span>
    </t-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';
import { MessagePlugin } from 'tdesign-vue-next';
import { AddIcon, SearchIcon } from 'tdesign-icons-vue-next';
import {
  createApi,
  deleteApi,
  getPageApi,
  resetPasswordApi,
  updateApi,
} from '@/api/user';
import type { UserItem } from '@/types';
import { authState } from '@/stores/auth';

const columns = [
  { colKey: 'userName', title: '用户名', width: 150 },
  { colKey: 'displayName', title: '显示名', width: 150 },
  { colKey: 'status', title: '状态', width: 90 },
  { colKey: 'lastLoginAt', title: '最近登录', width: 180 },
  { colKey: 'updatedAt', title: '更新时间', width: 180 },
  { colKey: 'operation', title: '操作', width: 220, fixed: 'right' },
];

const loading = ref(false);
const rows = ref<UserItem[]>([]);
const total = ref(0);
const pageIndex = ref(1);
const pageSize = ref(10);
const keyword = ref('');

const currentUserName = computed(() => authState.userName);

function isAdmin(row: UserItem | null): boolean {
  return row !== null && row.userName.toLowerCase() === 'admin';
}

function isSelf(row: UserItem | null): boolean {
  return row !== null && row.userName === currentUserName.value;
}

/** admin 与当前登录账号：状态开关锁定（后端同样拦截） */
function isStatusLocked(row: UserItem | null): boolean {
  return isAdmin(row) || isSelf(row);
}

function canDelete(row: UserItem): boolean {
  return !isAdmin(row) && !isSelf(row);
}

async function loadData() {
  loading.value = true;
  try {
    const result = await getPageApi({
      keyword: keyword.value.trim(),
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
    });
    rows.value = result.list;
    total.value = result.total;
  } catch (error) {
    MessagePlugin.error((error as Error).message || '加载失败');
  } finally {
    loading.value = false;
  }
}

const onSearch = () => {
  pageIndex.value = 1;
  loadData();
};

const onReset = () => {
  keyword.value = '';
  pageIndex.value = 1;
  loadData();
};

/* ------- 新增 / 编辑 ------- */
const dialogVisible = ref(false);
const submitting = ref(false);
const editingId = ref<number | null>(null);
const editRow = ref<UserItem | null>(null);

const emptyForm = () => ({
  userName: '',
  displayName: '',
  password: '',
  status: 1,
});
const form = reactive(emptyForm());

const openCreate = () => {
  editingId.value = null;
  editRow.value = null;
  Object.assign(form, emptyForm());
  dialogVisible.value = true;
};

const openEdit = (row: UserItem) => {
  editingId.value = row.id;
  editRow.value = row;
  Object.assign(form, {
    userName: row.userName,
    displayName: row.displayName,
    password: '',
    status: row.status,
  });
  dialogVisible.value = true;
};

const closeDialog = () => {
  if (!submitting.value) {
    dialogVisible.value = false;
  }
};

const submitForm = async () => {
  if (editingId.value === null) {
    if (!form.userName.trim()) {
      MessagePlugin.warning('用户名不能为空');
      return;
    }
    if (!form.password) {
      MessagePlugin.warning('初始密码不能为空');
      return;
    }
    if (form.password.length < 3) {
      MessagePlugin.warning('密码长度至少 3 位');
      return;
    }
  }

  submitting.value = true;
  try {
    if (editingId.value === null) {
      await createApi({
        userName: form.userName.trim(),
        password: form.password,
        displayName: form.displayName.trim() || undefined,
      });
      MessagePlugin.success('新增成功');
    } else {
      const payload: { displayName?: string; status?: number } = {};
      if (form.displayName.trim()) {
        payload.displayName = form.displayName.trim();
      }
      if (!isStatusLocked(editRow.value)) {
        payload.status = form.status;
      }
      await updateApi(editingId.value, payload);
      MessagePlugin.success('保存成功');
    }
    dialogVisible.value = false;
    await loadData();
  } catch (error) {
    MessagePlugin.error((error as Error).message || '保存失败');
  } finally {
    submitting.value = false;
  }
};

/** 状态开关：在表格上直接启用/禁用 */
const toggleStatus = async (row: UserItem, checked: boolean) => {
  const target = checked ? 1 : 0;
  if (target === row.status) {
    return;
  }
  try {
    await updateApi(row.id, { status: target });
    MessagePlugin.success(target === 1 ? '已启用' : '已禁用');
    await loadData();
  } catch (error) {
    MessagePlugin.error((error as Error).message || '操作失败');
    await loadData();
  }
};

/* ------- 重置密码 ------- */
const resetVisible = ref(false);
const resetting = ref(false);
const resetTarget = ref<UserItem | null>(null);
const resetForm = reactive({ password: '', confirm: '' });

const openReset = (row: UserItem) => {
  resetTarget.value = row;
  resetForm.password = '';
  resetForm.confirm = '';
  resetVisible.value = true;
};

const closeReset = () => {
  if (!resetting.value) {
    resetVisible.value = false;
    resetTarget.value = null;
  }
};

const confirmReset = async () => {
  if (!resetTarget.value) {
    return;
  }
  if (!resetForm.password) {
    MessagePlugin.warning('请输入新密码');
    return;
  }
  if (resetForm.password.length < 3) {
    MessagePlugin.warning('密码长度至少 3 位');
    return;
  }
  if (resetForm.password !== resetForm.confirm) {
    MessagePlugin.warning('两次输入的密码不一致');
    return;
  }
  resetting.value = true;
  try {
    await resetPasswordApi(resetTarget.value.id, resetForm.password);
    MessagePlugin.success('密码已重置');
    resetVisible.value = false;
    resetTarget.value = null;
  } catch (error) {
    MessagePlugin.error((error as Error).message || '重置失败');
  } finally {
    resetting.value = false;
  }
};

/* ------- 删除 ------- */
const deleteVisible = ref(false);
const deleting = ref(false);
const deleteTarget = ref<UserItem | null>(null);

const openDelete = (row: UserItem) => {
  deleteTarget.value = row;
  deleteVisible.value = true;
};

const closeDelete = () => {
  if (!deleting.value) {
    deleteVisible.value = false;
    deleteTarget.value = null;
  }
};

const confirmDelete = async () => {
  if (!deleteTarget.value) {
    return;
  }
  deleting.value = true;
  try {
    await deleteApi(deleteTarget.value.id);
    MessagePlugin.success('删除成功');
    if (rows.value.length === 1 && pageIndex.value > 1) {
      pageIndex.value -= 1;
    }
    deleteVisible.value = false;
    deleteTarget.value = null;
    await loadData();
  } catch (error) {
    MessagePlugin.error((error as Error).message || '删除失败');
  } finally {
    deleting.value = false;
  }
};

onMounted(loadData);
</script>

<style scoped>
.user-page {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.toolbar-card :deep(.t-card__body),
.list-card :deep(.t-card__body) {
  padding: 16px;
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
}

.keyword-input {
  width: 300px;
}

.spacer {
  flex: 1;
}

.pagination {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
}

.form-tip {
  font-size: 12px;
  color: #999;
}
</style>
