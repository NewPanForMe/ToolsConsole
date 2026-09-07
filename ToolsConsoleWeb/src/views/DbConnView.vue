<template>
  <div class="dbconn-page">
    <!-- 工具栏 -->
    <t-card :bordered="false" class="toolbar-card">
      <div class="toolbar">
        <t-input
          v-model="keyword"
          class="keyword-input"
          placeholder="按 ConnName / 主机 / 库名 搜索"
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
          新增连接配置
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
          <t-tag :theme="row.status === 1 ? 'success' : 'default'" variant="light">
            {{ row.status === 1 ? '启用' : '禁用' }}
          </t-tag>
        </template>
        <template #operation="{ row }">
          <t-space>
            <t-button size="small" variant="text" theme="primary" @click="openConnString(row)">
              获取连接串
            </t-button>
            <t-button size="small" variant="text" theme="default" @click="openEdit(row)">编辑</t-button>
            <t-button size="small" variant="text" theme="danger" @click="onDelete(row)">删除</t-button>
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

    <!-- 新增/编辑 对话框 -->
    <t-dialog
      :visible="dialogVisible"
      :header="editingId ? '编辑连接配置' : '新增连接配置'"
      :width="620"
      cancel-btn="取消"
      confirm-btn="保存"
      :on-cancel="closeDialog"
      :on-close="closeDialog"
      :on-overlay-click="closeDialog"
      :on-confirm="submitForm"
      :confirm-loading="submitting"
    >
      <t-form :data="form" label-align="right">
        <t-form-item label="ConnName" name="connName" required-mark>
          <t-input v-model="form.connName" placeholder="连接名称，全局唯一（取串时使用）" />
        </t-form-item>
        <t-form-item label="数据库类型" name="dbType">
          <t-select v-model="form.dbType" :options="dbTypeOptions" />
        </t-form-item>
        <t-form-item label="Host" name="host" required-mark>
          <t-input v-model="form.host" placeholder="主机地址，如 47.102.153.9" />
        </t-form-item>
        <t-form-item label="Port" name="port">
          <t-input-number v-model="form.port" :min="1" :max="65535" theme="column" />
        </t-form-item>
        <t-form-item label="Database" name="database" required-mark>
          <t-input v-model="form.database" placeholder="数据库名" />
        </t-form-item>
        <t-form-item label="Username" name="username" required-mark>
          <t-input v-model="form.username" placeholder="登录用户名" />
        </t-form-item>
        <t-form-item label="密码" name="password">
          <t-input
            v-model="form.password"
            type="password"
            :placeholder="editingId ? '留空表示不修改密码' : '数据库密码（AES 加密后存储）'"
            clearable
          />
        </t-form-item>
        <t-form-item label="附加参数" name="options">
          <t-textarea
            v-model="form.options"
            :autosize="{ minRows: 1, maxRows: 3 }"
            placeholder="可选，如 SSL Mode=Disable"
          />
        </t-form-item>
        <t-form-item label="备注" name="remark">
          <t-textarea
            v-model="form.remark"
            :autosize="{ minRows: 1, maxRows: 3 }"
            placeholder="可选备注"
          />
        </t-form-item>
      </t-form>
    </t-dialog>

    <!-- 取串结果 对话框（明文不出网，返回密文 + 解密说明） -->
    <t-dialog
      :visible="connVisible"
      header="按 ConnName 获取链接串（返回已加密）"
      cancel-btn=""
      confirm-btn="关闭"
      :on-confirm="closeConn"
      :on-close="closeConn"
      :on-overlay-click="closeConn"
      :width="760"
    >
      <div class="conn-box">
        <div class="conn-meta">
          ConnName：<b>{{ connResult.connName }}</b>&nbsp;&nbsp;DbType：<b>{{ connResult.dbType }}</b>&nbsp;&nbsp;算法：<b>{{ connResult.algorithm }}</b>
        </div>
        <t-textarea
          v-model="connResult.connectionStringEncrypted"
          :autosize="{ minRows: 3, maxRows: 6 }"
          readonly
        />
        <div class="conn-hint">
          <b>解密方式：</b>{{ connResult.decryptInstructions }}
        </div>
        <div class="conn-actions">
          <t-button theme="primary" variant="outline" size="small" @click="copyConnString">复制密文</t-button>
        </div>
      </div>
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
      <span>确定删除连接配置「<b>{{ deleteTarget?.connName }}</b>」吗？删除后不可恢复。</span>
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
  getConnectionStringApi,
  getPageApi,
  updateApi,
} from '@/api/dbconn';
import type { ConnectionStringDelivery, DbConnItem, DbConnSavePayload } from '@/types';
import { DB_TYPES } from '@/types';

const dbTypeOptions = computed(() =>
  DB_TYPES.map((item) => ({
    label: item.value === 'PostgreSQL' ? item.label : `${item.label}（暂不支持组装）`,
    value: item.value,
    disabled: item.value !== 'PostgreSQL',
  })),
);

const columns = [
  { colKey: 'connName', title: 'ConnName', width: 160, ellipsis: true },
  { colKey: 'dbType', title: '数据库类型', width: 110 },
  { colKey: 'host', title: 'Host', width: 150, ellipsis: true },
  { colKey: 'port', title: 'Port', width: 90 },
  { colKey: 'database', title: 'Database', width: 150, ellipsis: true },
  { colKey: 'username', title: 'Username', width: 120, ellipsis: true },
  { colKey: 'options', title: '附加参数', width: 150, ellipsis: true },
  { colKey: 'status', title: '状态', width: 80 },
  { colKey: 'updatedAt', title: '更新时间', width: 180 },
  { colKey: 'operation', title: '操作', width: 220, fixed: 'right' },
];

const loading = ref(false);
const rows = ref<DbConnItem[]>([]);
const total = ref(0);
const pageIndex = ref(1);
const pageSize = ref(10);
const keyword = ref('');

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

const emptyForm = () => ({
  connName: '',
  dbType: 'PostgreSQL',
  host: '',
  port: 5432,
  database: '',
  username: '',
  password: '',
  options: '',
  remark: '',
});
const form = reactive(emptyForm());

const openCreate = () => {
  editingId.value = null;
  Object.assign(form, emptyForm());
  dialogVisible.value = true;
};

const openEdit = (row: DbConnItem) => {
  editingId.value = row.id;
  Object.assign(form, {
    connName: row.connName,
    dbType: row.dbType,
    host: row.host,
    port: row.port,
    database: row.database,
    username: row.username,
    password: '',
    options: row.options ?? '',
    remark: row.remark ?? '',
  });
  dialogVisible.value = true;
};

const closeDialog = () => {
  if (!submitting.value) {
    dialogVisible.value = false;
  }
};

const submitForm = async () => {
  if (!form.connName.trim()) {
    MessagePlugin.warning('ConnName 不能为空');
    return;
  }
  if (!form.host.trim()) {
    MessagePlugin.warning('Host 不能为空');
    return;
  }
  if (!form.database.trim()) {
    MessagePlugin.warning('Database 不能为空');
    return;
  }
  if (!form.username.trim()) {
    MessagePlugin.warning('Username 不能为空');
    return;
  }

  submitting.value = true;
  try {
    const payload: DbConnSavePayload = {
      connName: form.connName.trim(),
      dbType: form.dbType,
      host: form.host.trim(),
      port: form.port,
      database: form.database.trim(),
      username: form.username.trim(),
      options: form.options.trim() || null,
      remark: form.remark.trim() || null,
    };
    if (editingId.value === null) {
      payload.password = form.password || null;
      await createApi(payload);
      MessagePlugin.success('新增成功');
    } else {
      // 编辑：密码留空表示不修改
      if (form.password) {
        payload.password = form.password;
      }
      payload.status = 1;
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

/* ------- 删除 ------- */
const deleteVisible = ref(false);
const deleting = ref(false);
const deleteTarget = ref<DbConnItem | null>(null);

const onDelete = (row: DbConnItem) => {
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

/* ------- 取串 ------- */
const connVisible = ref(false);
const connResult = reactive<ConnectionStringDelivery>({
  connName: '',
  dbType: '',
  connectionStringEncrypted: '',
  algorithm: '',
  keySource: '',
  payloadLayout: '',
  decryptInstructions: '',
});

const openConnString = async (row: DbConnItem) => {
  try {
    const result = await getConnectionStringApi(row.connName);
    Object.assign(connResult, result);
    connVisible.value = true;
  } catch (error) {
    MessagePlugin.error((error as Error).message || '获取链接串失败');
  }
};

const closeConn = () => {
  connVisible.value = false;
};

const copyConnString = async () => {
  try {
    await navigator.clipboard.writeText(connResult.connectionStringEncrypted);
    MessagePlugin.success('密文已复制到剪贴板');
  } catch {
    MessagePlugin.error('复制失败，请手动选择复制');
  }
};

onMounted(loadData);
</script>

<style scoped>
.dbconn-page {
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
  width: 320px;
}

.spacer {
  flex: 1;
}

.pagination {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
}

.conn-meta {
  margin-bottom: 8px;
  font-size: 13px;
  color: #666;
}

.conn-hint {
  margin-top: 8px;
  font-size: 12px;
  line-height: 1.6;
  color: #888;
  word-break: break-all;
}

.conn-actions {
  margin-top: 8px;
  text-align: right;
}
</style>
