<script setup lang="ts">
import { ref, computed } from 'vue'
import { siteConfig } from '../config/siteConfig'

const { techStack, techCategories, colors } = siteConfig

const activeCategory = ref<string>('all')

const filteredTechStack = computed(() => {
  if (activeCategory.value === 'all') {
    return techStack
  }
  return techStack.filter(item => item.category === activeCategory.value)
})

const categories = techCategories.map(cat => ({
  value: cat.key,
  label: cat.key === 'all' ? '全部技术' : cat.key === 'frontend' ? '前端技术' : cat.key === 'backend' ? '后端技术' : '其他技术'
}))

const getCategoryColor = (category: string) => {
  switch (category) {
    case 'frontend': return colors.categoryFrontend
    case 'backend': return colors.categoryBackend
    case 'other': return colors.categoryOther
    default: return colors.textLight
  }
}
</script>

<template>
  <div class="tech-stack">
    <div class="filter-section">
      <h2>技术分类</h2>
      <div class="filter-buttons">
        <button
          v-for="category in categories"
          :key="category.value"
          :class="['filter-btn', { active: activeCategory === category.value }]"
          @click="activeCategory = category.value"
        >
          {{ category.label }}
        </button>
      </div>
    </div>

    <div class="tech-grid">
      <div
        v-for="(tech, index) in filteredTechStack"
        :key="tech.id"
        class="tech-card"
        :style="{
          borderLeft: `4px solid ${getCategoryColor(tech.category)}`,
          animationDelay: `${index * 0.1}s`
        }"
      >
        <div class="card-header">
          <span class="tech-icon">{{ tech.icon }}</span>
          <h3>{{ tech.name }}</h3>
        </div>

        <p class="tech-description">{{ tech.description }}</p>

        <div class="tech-tags">
          <span
            v-for="tag in tech.tags"
            :key="tag"
            class="tag"
            :style="{ backgroundColor: getCategoryColor(tech.category) + '20', color: getCategoryColor(tech.category) }"
          >
            {{ tag }}
          </span>
        </div>

        <div class="category-badge" :style="{ backgroundColor: getCategoryColor(tech.category) }">
          {{ tech.category === 'frontend' ? '前端' : tech.category === 'backend' ? '后端' : '其他' }}
        </div>
      </div>
    </div>

    <div class="stats-section">
      <h2>技术栈统计</h2>
      <div class="stats-grid">
        <div class="stat-item">
          <div class="stat-number">{{ techStack.length }}</div>
          <div class="stat-label">技术总数</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ techStack.filter(t => t.category === 'frontend').length }}</div>
          <div class="stat-label">前端技术</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ techStack.filter(t => t.category === 'backend').length }}</div>
          <div class="stat-label">后端技术</div>
        </div>
        <div class="stat-item">
          <div class="stat-number">{{ techStack.filter(t => t.category === 'other').length }}</div>
          <div class="stat-label">其他技术</div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.tech-stack {
  padding: 1rem 0;
}

.filter-section {
  margin-bottom: 2rem;
}

.filter-section h2 {
  color: #2c3e50;
  margin-bottom: 1rem;
  font-size: 1.5rem;
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

.tech-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.tech-card {
  background: linear-gradient(135deg, #ffffff 0%, #f1f8e9 100%);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  position: relative;
  overflow: hidden;
  animation: cardFadeIn 0.6s ease-out forwards;
  opacity: 0;
  transform: translateY(20px);
}

@keyframes cardFadeIn {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.tech-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(46, 204, 113, 0.15);
}

.card-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.tech-icon {
  font-size: 2rem;
  width: 50px;
  height: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);
  border-radius: 10px;
}

.card-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.2rem;
  font-weight: 600;
  line-height: 1.3;
}

.tech-description {
  color: #666;
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

.tag {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
}

.category-badge {
  position: absolute;
  top: 1rem;
  right: 1rem;
  padding: 0.25rem 0.75rem;
  border-radius: 15px;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
}

.stats-section {
  background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);
  padding: 2rem;
  border-radius: 12px;
  margin-top: 2rem;
}

.stats-section h2 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
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
  font-size: 2.5rem;
  font-weight: 700;
  color: #2ecc71;
  margin-bottom: 0.5rem;
}

.stat-label {
  color: #666;
  font-size: 0.9rem;
  font-weight: 500;
}

@media (max-width: 768px) {
  .tech-grid {
    grid-template-columns: 1fr;
  }

  .filter-buttons {
    justify-content: center;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
