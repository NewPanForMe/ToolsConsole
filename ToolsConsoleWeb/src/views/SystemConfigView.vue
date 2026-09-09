<template>
  <div class="system-config-page">
    <t-card :bordered="false" class="toolbar-card">
      <div class="toolbar">
        <t-input
          v-model="keyword"
          class="keyword-input"
          placeholder="按配置 Key / 备注搜索"
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
          新增配置
        </t-button>
      </div>
    </t-card>

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
        <template #configValue="{ row }">
          <span class="secret-value">{{ maskValue(row.configValue) }}</span>
        </template>
        <template #updatedAt="{ row }">
          {{ formatDateTime(row.updatedAt) }}
        </template>
        <template #operation="{ row }">
          <t-button size="small" variant="text" theme="primary" @click="openEdit(row)">
            编辑
          </t-button>
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

    <t-dialog
      :visible="dialogVisible"
      :header="editingId ? '编辑系统配置' : '新增系统配置'"
      :width="640"
      cancel-btn="取消"
      confirm-btn="保存"
      :on-cancel="closeDialog"
      :on-close="closeDialog"
      :on-overlay-click="closeDialog"
      :on-confirm="submitForm"
      :confirm-loading="submitting"
    >
      <t-form :data="form" label-align="right">
        <t-form-item label="配置 Key" name="configKey" required-mark>
          <t-input
            v-model="form.configKey"
            :disabled="editingId !== null"
            placeholder="如 Security:DeliveryKey"
          />
        </t-form-item>
        <t-form-item label="配置值" name="configValue" required-mark>
          <t-textarea
            v-if="showValue"
            v-model="form.configValue"
            :autosize="{ minRows: 3, maxRows: 6 }"
            placeholder="配置值"
          />
          <t-input
            v-else
            :value="maskValue(form.configValue)"
            readonly
            placeholder="配置值"
          />
          <div class="value-actions">
            <t-button size="small" variant="text" theme="primary" @click="showValue = !showValue">
              {{ showValue ? '隐藏配置值' : '显示配置值' }}
            </t-button>
          </div>
        </t-form-item>
        <t-form-item label="备注" name="remark">
          <t-textarea
            v-model="form.remark"
            :autosize="{ minRows: 2, maxRows: 4 }"
            placeholder="可选备注"
          />
        </t-form-item>
      </t-form>
    </t-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { MessagePlugin } from 'tdesign-vue-next';
import { AddIcon, SearchIcon } from 'tdesign-icons-vue-next';
import { createApi, getPageApi, updateApi } from '@/api/system-config';
import type { SystemConfigItem } from '@/types';

const columns = [
  { colKey: 'configKey', title: '配置 Key', width: 220, ellipsis: true },
  { colKey: 'configValue', title: '配置值', width: 260, ellipsis: true },
  { colKey: 'remark', title: '备注', width: 220, ellipsis: true },
  { colKey: 'updatedAt', title: '更新时间', width: 180 },
  { colKey: 'operation', title: '操作', width: 90, fixed: 'right' },
];

const loading = ref(false);
const rows = ref<SystemConfigItem[]>([]);
const total = ref(0);
const pageIndex = ref(1);
const pageSize = ref(10);
const keyword = ref('');

function maskValue(value: string): string {
  if (!value) {
    return '';
  }

  if (value.length <= 8) {
    return '*'.repeat(value.length);
  }

  return `${value.slice(0, 4)}${'*'.repeat(Math.min(value.length - 8, 24))}${value.slice(-4)}`;
}

function formatDateTime(value: string | null | undefined): string {
  if (!value) {
    return '-';
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return '-';
  }

  const pad = (num: number) => num.toString().padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} `
    + `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
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

const dialogVisible = ref(false);
const submitting = ref(false);
const editingId = ref<number | null>(null);
const showValue = ref(false);

const emptyForm = () => ({
  configKey: '',
  configValue: '',
  remark: '',
});
const form = reactive(emptyForm());

const openCreate = () => {
  editingId.value = null;
  showValue.value = true;
  Object.assign(form, emptyForm());
  dialogVisible.value = true;
};

const openEdit = (row: SystemConfigItem) => {
  editingId.value = row.id;
  showValue.value = false;
  Object.assign(form, {
    configKey: row.configKey,
    configValue: row.configValue,
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
  if (!form.configKey.trim()) {
    MessagePlugin.warning('配置 Key 不能为空');
    return;
  }
  if (!form.configValue) {
    MessagePlugin.warning('配置值不能为空');
    return;
  }

  submitting.value = true;
  try {
    if (editingId.value === null) {
      await createApi({
        configKey: form.configKey.trim(),
        configValue: form.configValue,
        remark: form.remark.trim() || null,
      });
      MessagePlugin.success('新增成功');
    } else {
      await updateApi(editingId.value, {
        configValue: form.configValue,
        remark: form.remark.trim() || null,
      });
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

onMounted(loadData);
</script>

<style scoped>
.system-config-page {
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

.secret-value {
  font-family: ui-monospace, SFMono-Regular, Consolas, 'Liberation Mono', monospace;
  color: #4b5563;
}

.value-actions {
  margin-top: 6px;
  text-align: right;
}

.pagination {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
}
</style>
