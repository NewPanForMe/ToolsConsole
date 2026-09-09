<script setup lang="ts">
import { ref, computed } from 'vue'
import type { CategoryOption, ProjectItem, SiteColors } from '@/types/site-config'

const props = defineProps<{
  projects: ProjectItem[]
  projectCategories: CategoryOption[]
  colors: SiteColors
}>()

const activeCategory = ref<string>('all')

const filteredProjects = computed(() => {
  if (activeCategory.value === 'all') {
    return props.projects
  }
  return props.projects.filter(project => project.category === activeCategory.value)
})

const getCategoryColor = (category: string) => {
  switch (category) {
    case 'enterprise': return props.colors.primary
    case 'mobile': return props.colors.categoryBackend
    case 'visualization': return '#9b59b6'
    case 'security': return props.colors.categoryOther
    default: return props.colors.textLight
  }
}

const expandedProject = ref<number | null>(null)

const toggleExpand = (id: number) => {
  expandedProject.value = expandedProject.value === id ? null : id
}
</script>

<template>
  <div class="project-experience">
    <!-- 分类筛选 -->
    <div class="filter-section">
      <div class="filter-buttons">
        <button
          v-for="category in projectCategories"
          :key="category.key"
          :class="['filter-btn', { active: activeCategory === category.key }]"
          @click="activeCategory = category.key"
        >
          {{ category.label }}
        </button>
      </div>
    </div>

    <!-- 项目时间线 -->
    <div class="timeline">
      <div
        v-for="(project, index) in filteredProjects"
        :key="project.id"
        class="timeline-item"
        :style="{ animationDelay: `${index * 0.15}s` }"
      >
        <!-- 时间线节点 -->
        <div
          class="timeline-dot"
          :style="{ backgroundColor: getCategoryColor(project.category) }"
        />

        <!-- 项目卡片 -->
        <div
          class="project-card"
          :class="{ expanded: expandedProject === project.id }"
          @click="toggleExpand(project.id)"
        >
          <!-- 卡片头部 -->
          <div class="card-header">
            <div class="header-left">
              <div class="period-badge" :style="{ backgroundColor: getCategoryColor(project.category) }">
                {{ project.period }}
              </div>
              <h3 class="project-name">{{ project.name }}</h3>
              <div class="project-meta">
                <span class="role">{{ project.role }}</span>
                <span class="separator">•</span>
                <span class="company">{{ project.company }}</span>
              </div>
            </div>
            <div class="expand-icon">
              <span :class="{ rotated: expandedProject === project.id }">▼</span>
            </div>
          </div>

          <!-- 项目简介 -->
          <p class="project-description">{{ project.description }}</p>

          <!-- 技术标签 -->
          <div class="tech-tags">
            <span
              v-for="tech in project.technologies"
              :key="tech"
              class="tech-tag"
              :style="{
                backgroundColor: getCategoryColor(project.category) + '15',
                color: getCategoryColor(project.category),
                borderColor: getCategoryColor(project.category) + '30',
              }"
            >
              {{ tech }}
            </span>
          </div>

          <!-- 展开内容 -->
          <div v-if="expandedProject === project.id" class="expanded-content">
            <!-- 工作职责 -->
            <div class="section">
              <h4 class="section-title">
                <span class="section-icon">📋</span>
                工作职责
              </h4>
              <ul class="responsibility-list">
                <li v-for="(resp, idx) in project.responsibilities" :key="idx">
                  {{ resp }}
                </li>
              </ul>
            </div>

            <!-- 项目成果 -->
            <div class="section">
              <h4 class="section-title">
                <span class="section-icon">🏆</span>
                项目成果
              </h4>
              <ul class="achievement-list">
                <li v-for="(achievement, idx) in project.achievements" :key="idx">
                  {{ achievement }}
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 项目统计 -->
    <div class="stats-section">
      <h3>项目统计</h3>
      <div class="stats-grid">
        <div class="stat-item">
          <div class="stat-number">{{ props.projects.length }}</div>
          <div class="stat-label">项目总数</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ new Set(props.projects.flatMap(p => p.technologies)).size }}</div>
          <div class="stat-label">涉及技术</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ props.projects.reduce((sum, p) => sum + p.achievements.length, 0) }}</div>
          <div class="stat-label">项目成果</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ props.projectCategories.length - 1 }}</div>
          <div class="stat-label">项目类型</div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.project-experience {
  padding: 1rem 0;
}

.filter-section {
  margin-bottom: 2rem;
}

.filter-buttons {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.filter-btn {
  padding: 0.5rem 1rem;
  border: 2px solid #c8e6c9;
  background: linear-gradient(135deg, #ffffff 0%, #e8f5e9 100%);
  border-radius: 25px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 0.9rem;
  color: #666;
}

.filter-btn:hover {
  border-color: #2ecc71;
  color: #2ecc71;
}

.filter-btn.active {
  background: #2ecc71;
  border-color: #2ecc71;
  color: white;
}

/* 时间线样式 */
.timeline {
  position: relative;
  padding-left: 2rem;
}

.timeline::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 2px;
  background: linear-gradient(180deg, #c8e6c9 0%, #a5d6a7 100%);
}

.timeline-item {
  position: relative;
  margin-bottom: 2rem;
  animation: fadeInLeft 0.6s ease-out forwards;
  opacity: 0;
  transform: translateX(-20px);
}

@keyframes fadeInLeft {
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.timeline-dot {
  position: absolute;
  left: -2.35rem;
  top: 1.5rem;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: 3px solid white;
  box-shadow: 0 0 0 3px #c8e6c9;
  z-index: 1;
}

/* 项目卡片样式 */
.project-card {
  background: linear-gradient(135deg, #ffffff 0%, #f1f8e9 100%);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s ease;
  border-left: 4px solid transparent;
}

.project-card:hover {
  box-shadow: 0 8px 25px rgba(46, 204, 113, 0.15);
  transform: translateX(5px);
}

.project-card.expanded {
  box-shadow: 0 8px 30px rgba(46, 204, 113, 0.2);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.header-left {
  flex: 1;
}

.period-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 15px;
  color: white;
  font-size: 0.8rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
}

.project-name {
  font-size: 1.3rem;
  color: #2c3e50;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.project-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #666;
  font-size: 0.95rem;
}

.separator {
  color: #ccc;
}

.expand-icon {
  color: #999;
  transition: transform 0.3s ease;
  font-size: 0.8rem;
}

.expand-icon .rotated {
  display: inline-block;
  transform: rotate(180deg);
}

.project-description {
  color: #555;
  line-height: 1.6;
  margin-bottom: 1rem;
  font-size: 0.95rem;
}

.tech-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.tech-tag {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
  border: 1px solid;
}

/* 展开内容样式 */
.expanded-content {
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e8f5e9;
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.section {
  margin-bottom: 1.5rem;
}

.section:last-child {
  margin-bottom: 0;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #2c3e50;
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 1rem;
}

.section-icon {
  font-size: 1.2rem;
}

.responsibility-list,
.achievement-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.responsibility-list li,
.achievement-list li {
  position: relative;
  padding-left: 1.5rem;
  margin-bottom: 0.75rem;
  color: #555;
  line-height: 1.5;
  font-size: 0.95rem;
}

.responsibility-list li::before {
  content: '▸';
  position: absolute;
  left: 0;
  color: #2ecc71;
  font-weight: bold;
}

.achievement-list li::before {
  content: '✓';
  position: absolute;
  left: 0;
  color: #27ae60;
  font-weight: bold;
}

/* 统计区域 */
.stats-section {
  background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);
  padding: 2rem;
  border-radius: 12px;
  margin-top: 2rem;
}

.stats-section h3 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 1.5rem;
  font-size: 1.3rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.5rem;
}

.stat-item {
  text-align: center;
  padding: 1rem;
  background: linear-gradient(135deg, #ffffff 0%, #e8f5e9 100%);
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

.stat-number {
  font-size: 2rem;
  font-weight: 700;
  color: #2ecc71;
  margin-bottom: 0.5rem;
}

.stat-label {
  color: #666;
  font-size: 0.85rem;
  font-weight: 500;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .timeline {
    padding-left: 1.5rem;
  }

  .timeline-dot {
    left: -1.85rem;
    width: 12px;
    height: 12px;
  }

  .project-card {
    padding: 1rem;
  }

  .card-header {
    flex-direction: column;
    gap: 0.5rem;
  }

  .expand-icon {
    align-self: flex-end;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .filter-buttons {
    justify-content: center;
  }
}
</style>
