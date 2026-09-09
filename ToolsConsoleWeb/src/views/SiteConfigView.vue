<template>
  <div class="site-config-page">
    <t-card :bordered="false" class="toolbar-card">
      <div class="toolbar">
        <div>
          <div class="page-heading">站点配置</div>
          <div class="page-subtitle">配置 ToolsConsoleIndex 中可重复展示的内容</div>
        </div>
        <div class="spacer" />
        <t-button theme="default" :loading="loading" @click="loadAll">刷新</t-button>
        <t-button theme="primary" :loading="saving" @click="saveActiveItems">保存当前配置</t-button>
      </div>
    </t-card>

    <t-card :bordered="false" class="list-card">
      <t-tabs v-model="activeTab">
        <t-tab-panel value="techStack" label="技术栈">
          <div class="tab-actions">
            <t-button theme="primary" @click="openCreateTech">新增技术栈</t-button>
            <span class="count-text">{{ activeCountText }}</span>
          </div>
          <t-table
            :data="techStack"
            :columns="techColumns"
            :loading="loading"
            row-key="id"
            table-layout="fixed"
            hover
          >
            <template #tags="{ row }">
              <t-tag v-for="tag in row.tags" :key="tag" class="tag-item" theme="primary" variant="light">
                {{ tag }}
              </t-tag>
            </template>
            <template #operation="{ row }">
              <t-space>
                <t-button size="small" variant="text" theme="primary" @click="openEditTech(row)">编辑</t-button>
                <t-button size="small" variant="text" theme="danger" @click="removeTech(row.id)">删除</t-button>
              </t-space>
            </template>
          </t-table>
        </t-tab-panel>

        <t-tab-panel value="projects" label="项目经历">
          <div class="tab-actions">
            <t-button theme="primary" @click="openCreateProject">新增项目经历</t-button>
            <span class="count-text">{{ activeCountText }}</span>
          </div>
          <t-table
            :data="projects"
            :columns="projectColumns"
            :loading="loading"
            row-key="id"
            table-layout="fixed"
            hover
          >
            <template #technologies="{ row }">
              <t-tag v-for="tech in row.technologies" :key="tech" class="tag-item" theme="primary" variant="light">
                {{ tech }}
              </t-tag>
            </template>
            <template #operation="{ row }">
              <t-space>
                <t-button size="small" variant="text" theme="primary" @click="openEditProject(row)">编辑</t-button>
                <t-button size="small" variant="text" theme="danger" @click="removeProject(row.id)">删除</t-button>
              </t-space>
            </template>
          </t-table>
        </t-tab-panel>
      </t-tabs>
    </t-card>

    <t-dialog
      :visible="techDialogVisible"
      :header="editingTechId === null ? '新增技术栈' : '编辑技术栈'"
      :width="680"
      cancel-btn="取消"
      confirm-btn="确定"
      :on-cancel="closeTechDialog"
      :on-close="closeTechDialog"
      :on-confirm="submitTech"
    >
      <t-form :data="techForm" label-align="right">
        <t-form-item label="名称" required-mark>
          <t-input v-model="techForm.name" placeholder="如 Vue 3 + Vite" />
        </t-form-item>
        <t-form-item label="分类" required-mark>
          <t-select v-model="techForm.category" :options="techCategoryOptions" />
        </t-form-item>
        <t-form-item label="图标">
          <t-input v-model="techForm.icon" placeholder="如 ⚡" />
        </t-form-item>
        <t-form-item label="描述" required-mark>
          <t-textarea v-model="techForm.description" :autosize="{ minRows: 3, maxRows: 5 }" />
        </t-form-item>
        <t-form-item label="标签">
          <t-textarea
            v-model="techForm.tagsText"
            :autosize="{ minRows: 3, maxRows: 5 }"
            placeholder="每行一个标签"
          />
        </t-form-item>
      </t-form>
    </t-dialog>

    <t-dialog
      :visible="projectDialogVisible"
      :header="editingProjectId === null ? '新增项目经历' : '编辑项目经历'"
      :width="760"
      cancel-btn="取消"
      confirm-btn="确定"
      :on-cancel="closeProjectDialog"
      :on-close="closeProjectDialog"
      :on-confirm="submitProject"
    >
      <t-form :data="projectForm" label-align="right">
        <t-form-item label="项目名称" required-mark>
          <t-input v-model="projectForm.name" />
        </t-form-item>
        <t-form-item label="角色" required-mark>
          <t-input v-model="projectForm.role" />
        </t-form-item>
        <t-form-item label="周期" required-mark>
          <t-input v-model="projectForm.period" placeholder="如 2024.01 - 2024.06" />
        </t-form-item>
        <t-form-item label="公司">
          <t-input v-model="projectForm.company" />
        </t-form-item>
        <t-form-item label="分类" required-mark>
          <t-select v-model="projectForm.category" :options="projectCategoryOptions" />
        </t-form-item>
        <t-form-item label="项目简介" required-mark>
          <t-textarea v-model="projectForm.description" :autosize="{ minRows: 3, maxRows: 5 }" />
        </t-form-item>
        <t-form-item label="技术">
          <t-textarea v-model="projectForm.technologiesText" :autosize="{ minRows: 2, maxRows: 4 }" placeholder="每行一项" />
        </t-form-item>
        <t-form-item label="职责">
          <t-textarea v-model="projectForm.responsibilitiesText" :autosize="{ minRows: 3, maxRows: 5 }" placeholder="每行一项" />
        </t-form-item>
        <t-form-item label="成果">
          <t-textarea v-model="projectForm.achievementsText" :autosize="{ minRows: 3, maxRows: 5 }" placeholder="每行一项" />
        </t-form-item>
      </t-form>
    </t-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';
import { MessagePlugin } from 'tdesign-vue-next';
import { getByKeyApi, saveByKeyApi } from '@/api/system-config';
import type { SiteProjectItem, SiteTechStackItem } from '@/types';

const TECH_STACK_KEY = 'SiteConfig:TechStack';
const PROJECTS_KEY = 'SiteConfig:Projects';

type ActiveTab = 'techStack' | 'projects';

const activeTab = ref<ActiveTab>('techStack');
const loading = ref(false);
const saving = ref(false);
const techStack = ref<SiteTechStackItem[]>([]);
const projects = ref<SiteProjectItem[]>([]);

const techColumns = [
  { colKey: 'name', title: '名称', width: 220, ellipsis: true },
  { colKey: 'category', title: '分类', width: 100 },
  { colKey: 'icon', title: '图标', width: 80 },
  { colKey: 'description', title: '描述', width: 320, ellipsis: true },
  { colKey: 'tags', title: '标签', width: 240 },
  { colKey: 'operation', title: '操作', width: 140, fixed: 'right' },
];

const projectColumns = [
  { colKey: 'name', title: '项目名称', width: 180, ellipsis: true },
  { colKey: 'role', title: '角色', width: 140, ellipsis: true },
  { colKey: 'period', title: '周期', width: 150 },
  { colKey: 'company', title: '公司', width: 140, ellipsis: true },
  { colKey: 'category', title: '分类', width: 100 },
  { colKey: 'technologies', title: '技术', width: 260 },
  { colKey: 'operation', title: '操作', width: 140, fixed: 'right' },
];

const techCategoryOptions = [
  { label: '前端', value: 'frontend' },
  { label: '后端', value: 'backend' },
  { label: '其他', value: 'other' },
];

const projectCategoryOptions = [
  { label: '企业应用', value: 'enterprise' },
  { label: '移动端', value: 'mobile' },
  { label: '数据可视化', value: 'visualization' },
  { label: '安全相关', value: 'security' },
];

async function loadJsonConfig<T>(configKey: string): Promise<T[]> {
  try {
    const item = await getByKeyApi(configKey);
    const parsed = JSON.parse(item.configValue) as T[];
    return Array.isArray(parsed) ? parsed : [];
  } catch {
    return [];
  }
}

async function loadAll() {
  loading.value = true;
  try {
    const [loadedTechStack, loadedProjects] = await Promise.all([
      loadJsonConfig<SiteTechStackItem>(TECH_STACK_KEY),
      loadJsonConfig<SiteProjectItem>(PROJECTS_KEY),
    ]);
    techStack.value = loadedTechStack;
    projects.value = loadedProjects;
  } finally {
    loading.value = false;
  }
}

async function saveActiveItems() {
  saving.value = true;
  try {
    if (activeTab.value === 'techStack') {
      await saveByKeyApi({
        configKey: TECH_STACK_KEY,
        configValue: JSON.stringify(techStack.value, null, 2),
        remark: 'ToolsConsoleIndex 技术栈配置',
      });
    } else {
      await saveByKeyApi({
        configKey: PROJECTS_KEY,
        configValue: JSON.stringify(projects.value, null, 2),
        remark: 'ToolsConsoleIndex 项目经历配置',
      });
    }
    MessagePlugin.success('保存成功');
  } catch (error) {
    MessagePlugin.error((error as Error).message || '保存失败');
  } finally {
    saving.value = false;
  }
}

function splitLines(value: string): string[] {
  return value
    .split(/\r?\n/)
    .map((item) => item.trim())
    .filter(Boolean);
}

function nextId<T extends { id: number }>(items: T[]): number {
  return items.length === 0 ? 1 : Math.max(...items.map((item) => item.id)) + 1;
}

const techDialogVisible = ref(false);
const editingTechId = ref<number | null>(null);
const techForm = reactive({
  name: '',
  category: 'frontend',
  icon: '',
  description: '',
  tagsText: '',
});

const emptyTechForm = () => ({
  name: '',
  category: 'frontend',
  icon: '',
  description: '',
  tagsText: '',
});

function openCreateTech() {
  editingTechId.value = null;
  Object.assign(techForm, emptyTechForm());
  techDialogVisible.value = true;
}

function openEditTech(row: SiteTechStackItem) {
  editingTechId.value = row.id;
  Object.assign(techForm, {
    name: row.name,
    category: row.category,
    icon: row.icon,
    description: row.description,
    tagsText: row.tags.join('\n'),
  });
  techDialogVisible.value = true;
}

function closeTechDialog() {
  techDialogVisible.value = false;
}

function submitTech() {
  if (!techForm.name.trim() || !techForm.description.trim()) {
    MessagePlugin.warning('名称和描述不能为空');
    return;
  }

  const item: SiteTechStackItem = {
    id: editingTechId.value ?? nextId(techStack.value),
    name: techForm.name.trim(),
    category: techForm.category,
    icon: techForm.icon.trim() || '•',
    description: techForm.description.trim(),
    tags: splitLines(techForm.tagsText),
  };

  if (editingTechId.value === null) {
    techStack.value = [...techStack.value, item];
  } else {
    techStack.value = techStack.value.map((tech) => (tech.id === item.id ? item : tech));
  }
  techDialogVisible.value = false;
}

function removeTech(id: number) {
  techStack.value = techStack.value.filter((item) => item.id !== id);
}

const projectDialogVisible = ref(false);
const editingProjectId = ref<number | null>(null);
const projectForm = reactive({
  name: '',
  role: '',
  period: '',
  company: '',
  category: 'enterprise',
  description: '',
  technologiesText: '',
  responsibilitiesText: '',
  achievementsText: '',
});

const emptyProjectForm = () => ({
  name: '',
  role: '',
  period: '',
  company: '',
  category: 'enterprise',
  description: '',
  technologiesText: '',
  responsibilitiesText: '',
  achievementsText: '',
});

function openCreateProject() {
  editingProjectId.value = null;
  Object.assign(projectForm, emptyProjectForm());
  projectDialogVisible.value = true;
}

function openEditProject(row: SiteProjectItem) {
  editingProjectId.value = row.id;
  Object.assign(projectForm, {
    name: row.name,
    role: row.role,
    period: row.period,
    company: row.company,
    category: row.category,
    description: row.description,
    technologiesText: row.technologies.join('\n'),
    responsibilitiesText: row.responsibilities.join('\n'),
    achievementsText: row.achievements.join('\n'),
  });
  projectDialogVisible.value = true;
}

function closeProjectDialog() {
  projectDialogVisible.value = false;
}

function submitProject() {
  if (!projectForm.name.trim() || !projectForm.role.trim() || !projectForm.period.trim() || !projectForm.description.trim()) {
    MessagePlugin.warning('项目名称、角色、周期和简介不能为空');
    return;
  }

  const item: SiteProjectItem = {
    id: editingProjectId.value ?? nextId(projects.value),
    name: projectForm.name.trim(),
    role: projectForm.role.trim(),
    period: projectForm.period.trim(),
    company: projectForm.company.trim(),
    category: projectForm.category,
    description: projectForm.description.trim(),
    technologies: splitLines(projectForm.technologiesText),
    responsibilities: splitLines(projectForm.responsibilitiesText),
    achievements: splitLines(projectForm.achievementsText),
  };

  if (editingProjectId.value === null) {
    projects.value = [...projects.value, item];
  } else {
    projects.value = projects.value.map((project) => (project.id === item.id ? item : project));
  }
  projectDialogVisible.value = false;
}

function removeProject(id: number) {
  projects.value = projects.value.filter((item) => item.id !== id);
}

const activeCountText = computed(() => (activeTab.value === 'techStack' ? `${techStack.value.length} 项技术栈` : `${projects.value.length} 个项目`));

onMounted(loadAll);
</script>

<style scoped>
.site-config-page {
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
  gap: 12px;
}

.page-heading {
  font-size: 16px;
  font-weight: 700;
  color: #1f2937;
}

.page-subtitle {
  margin-top: 4px;
  color: #6b7280;
  font-size: 13px;
}

.spacer {
  flex: 1;
}

.tab-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 8px 0 12px;
}

.count-text {
  color: #6b7280;
  font-size: 13px;
}

.tag-item {
  margin: 0 6px 6px 0;
}
</style>
