<script setup lang="ts">
import TechStack from './components/TechStack.vue'
import ProjectExperience from './components/ProjectExperience.vue'
import { siteConfig } from './config/siteConfig'

const { personal, stats, socialLinks, contact, navLinks, pageTitle, skillsTitle, skillsSubtitle, projectsTitle, projectsSubtitle, contactTitle, footerText } = siteConfig
</script>

<template>
  <div class="personal-page">
    <!-- 顶部导航 -->
    <nav class="navbar">
      <div class="nav-brand">{{ pageTitle }}</div>
      <div class="nav-links">
        <a v-for="link in navLinks" :key="link.href" :href="link.href">{{ link.name }}</a>
      </div>
    </nav>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <!-- 个人介绍区域 -->
      <section id="about" class="about-section">
        <div class="about-container">
          <div class="avatar-section">
            <div class="avatar">
              <div class="avatar-placeholder">{{ personal.avatarText }}</div>
            </div>
            <h1 class="name">{{ personal.name }}</h1>
            <p class="title">{{ personal.title }}</p>
            <div class="social-links">
              <a v-for="link in socialLinks" :key="link.name" :href="link.url" class="social-link">{{ link.name }}</a>
            </div>
          </div>

          <div class="bio-section">
            <h2>关于我</h2>
            <p class="bio">{{ personal.bio }}</p>
            <div class="stats">
              <div v-for="(stat, index) in stats" :key="index" class="stat-item">
                <div class="stat-number">{{ stat.value }}</div>
                <div class="stat-label">{{ stat.label }}</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 技能展示区域 -->
      <section id="skills" class="skills-section">
        <div class="skills-container">
          <h2>{{ skillsTitle }}</h2>
          <p class="skills-subtitle">{{ skillsSubtitle }}</p>
          <TechStack />
        </div>
      </section>

      <!-- 项目经历区域 -->
      <section id="projects" class="projects-section">
        <div class="projects-container">
          <h2>{{ projectsTitle }}</h2>
          <p class="projects-subtitle">{{ projectsSubtitle }}</p>
          <ProjectExperience />
        </div>
      </section>

      <!-- 联系方式区域 -->
      <section id="contact" class="contact-section">
        <div class="contact-container">
          <h2>{{ contactTitle }}</h2>
          <div class="contact-info">
            <div class="contact-item">
              <strong>邮箱：</strong>{{ contact.email }}
            </div>
            <div class="contact-item">
              <strong>位置：</strong>{{ contact.location }}
            </div>
            <div class="contact-item">
              <strong>状态：</strong>{{ contact.status }}
            </div>
          </div>
        </div>
      </section>
    </div>

    <!-- 页脚 -->
    <footer class="footer">
      <p>{{ footerText }}</p>
    </footer>
  </div>
</template>

<style scoped>
.personal-page {
  height: 100vh;
  width: 100%;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
  overflow-x: hidden;
  background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 50%, #a5d6a7 100%);
}

/* 导航栏样式 */
.navbar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(20px);
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

.nav-brand {
  font-size: 1.5rem;
  font-weight: 700;
  color: #2c3e50;
}

.nav-links {
  display: flex;
  gap: 2rem;
}

.nav-links a {
  color: #666;
  text-decoration: none;
  font-weight: 500;
  transition: color 0.3s ease;
}

.nav-links a:hover {
  color: #2ecc71;
}

/* 主要内容区域 */
.main-content {
  flex: 1;
  margin-top: 80px;
}

/* 个人介绍区域 */
.about-section {
  padding: 4rem 2rem;
  background: linear-gradient(135deg, #f1f8e9 0%, #dcedc8 100%);
}

.about-container {
  max-width: 1200px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 4rem;
  align-items: center;
}

.avatar-section {
  text-align: center;
}

.avatar {
  width: 200px;
  height: 200px;
  margin: 0 auto 2rem;
  border-radius: 50%;
  overflow: hidden;
  background: linear-gradient(135deg, #2ecc71 0%, #1abc9c 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 10px 30px rgba(46, 204, 113, 0.3);
  animation: pulse 2s infinite;
  position: relative;
}

.avatar::before {
  content: '';
  position: absolute;
  top: -5px;
  left: -5px;
  right: -5px;
  bottom: -5px;
  border-radius: 50%;
  background: linear-gradient(135deg, #2ecc71 0%, #1abc9c 100%);
  z-index: -1;
  animation: pulse 2s infinite;
  opacity: 0.7;
}

@keyframes pulse {
  0% {
    transform: scale(1);
    box-shadow: 0 10px 30px rgba(46, 204, 113, 0.3);
  }
  50% {
    transform: scale(1.05);
    box-shadow: 0 15px 40px rgba(46, 204, 113, 0.4);
  }
  100% {
    transform: scale(1);
    box-shadow: 0 10px 30px rgba(46, 204, 113, 0.3);
  }
}

.avatar-placeholder {
  color: white;
  font-size: 1.2rem;
  font-weight: 600;
}

.name {
  font-size: 2.5rem;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.title {
  font-size: 1.2rem;
  color: #2ecc71;
  margin-bottom: 1.5rem;
  font-weight: 500;
}

.social-links {
  display: flex;
  justify-content: center;
  gap: 1rem;
}

.social-link {
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #ffffff 0%, #e8f5e9 100%);
  border-radius: 25px;
  color: #666;
  text-decoration: none;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.social-link:hover {
  background: #2ecc71;
  color: white;
}

.bio-section h2 {
  font-size: 2rem;
  color: #2c3e50;
  margin-bottom: 1.5rem;
}

.bio {
  font-size: 1.1rem;
  line-height: 1.8;
  color: #555;
  margin-bottom: 2rem;
}

.stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 2rem;
}

.stat-item {
  text-align: center;
  padding: 1.5rem;
  background: linear-gradient(135deg, #ffffff 0%, #e8f5e9 100%);
  border-radius: 10px;
  animation: fadeInUp 0.6s ease-out forwards;
  opacity: 0;
  transform: translateY(20px);
}

.stat-item:nth-child(1) { animation-delay: 0.1s; }
.stat-item:nth-child(2) { animation-delay: 0.2s; }
.stat-item:nth-child(3) { animation-delay: 0.3s; }

@keyframes fadeInUp {
  to {
    opacity: 1;
    transform: translateY(0);
  }
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
}

/* 技能展示区域 */
.skills-section {
  padding: 4rem 2rem;
  background: linear-gradient(135deg, #f1f8e9 0%, #dcedc8 100%);
}

.skills-container {
  max-width: 1200px;
  margin: 0 auto;
}

.skills-container h2 {
  font-size: 2rem;
  color: #2c3e50;
  text-align: center;
  margin-bottom: 0.5rem;
}

.skills-subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 3rem;
  font-size: 1.1rem;
}

/* 项目经历区域 */
.projects-section {
  padding: 4rem 2rem;
  background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);
}

.projects-container {
  max-width: 1200px;
  margin: 0 auto;
}

.projects-container h2 {
  font-size: 2rem;
  color: #2c3e50;
  text-align: center;
  margin-bottom: 0.5rem;
}

.projects-subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 3rem;
  font-size: 1.1rem;
}

/* 联系方式区域 */
.contact-section {
  padding: 4rem 2rem;
  background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);
}

.contact-container {
  max-width: 800px;
  margin: 0 auto;
  text-align: center;
}

.contact-container h2 {
  font-size: 2rem;
  color: #2c3e50;
  margin-bottom: 2rem;
}

.contact-info {
  display: flex;
  justify-content: center;
  gap: 3rem;
  flex-wrap: wrap;
}

.contact-item {
  font-size: 1.1rem;
  color: #555;
}

.contact-item strong {
  color: #2c3e50;
}

/* 页脚样式 */
.footer {
  background: linear-gradient(135deg, #2e7d32 0%, #388e3c 100%);
  color: white;
  text-align: center;
  padding: 1.5rem;
  font-size: 0.9rem;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .about-container {
    grid-template-columns: 1fr;
    gap: 2rem;
    text-align: center;
  }

  .stats {
    grid-template-columns: repeat(3, 1fr);
    gap: 1rem;
  }

  .nav-links {
    display: none;
  }

  .contact-info {
    flex-direction: column;
    gap: 1rem;
  }
}
</style>
