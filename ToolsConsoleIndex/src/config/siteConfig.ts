// 网站配置文件
export const siteConfig = {
  // 个人信息
  personal: {
    name: '开发者',
    title: '全栈开发工程师',
    avatarText: '头像+',
    bio: '热爱技术，专注于全栈开发。拥有丰富的前后端开发经验，熟悉现代Web技术栈，致力于构建高质量、可维护的软件系统。喜欢探索新技术，解决复杂问题，并与团队分享知识。',
  },

  // 统计数据
  stats: [
    { value: '5+', label: '年经验' },
    { value: '10+', label: '项目完成' },
    { value: '10+', label: '技术栈' },
  ],

  // 社交链接
  socialLinks: [
    { name: 'GitHub', url: 'https://github.com/NewPanForMe' },
    { name: 'QQ', url: '#' },
    { name: 'Tel', url: '17605254921' },
  ],

  // 联系方式
  contact: {
    email: 'yuans_70g@163.com',
    location: '中国江苏苏州',
    status: '在职-寻找兼职中',
  },

  // 导航链接
  navLinks: [
    { name: '关于我', href: '#about' },
    { name: '技能', href: '#skills' },
    { name: '项目经历', href: '#projects' },
    { name: '联系方式', href: '#contact' },
  ],

  // 页面标题
  pageTitle: '个人介绍',

  // 技能区域标题
  skillsTitle: '技术栈',
  skillsSubtitle: '我熟悉的技术和工具',

  // 联系区域标题
  contactTitle: '联系方式',

  // 页脚文本
  footerText: '基于 Vue 3 + Vite + TypeScript 构建 | 个人技术分享页面',

  // 技术栈数据
  techStack: [
    // 前端技术
    {
      id: 1,
      name: 'uni-app + Vue 3 + TypeScript + Pinia',
      description: '跨平台移动应用开发框架，支持App、H5、小程序等多端运行，使用Vue 3组合式API和TypeScript类型安全，Pinia状态管理。',
      category: 'frontend' as const,
      icon: '🖖',
      tags: ['跨平台', '移动开发', 'Vue 3', 'TypeScript'],
    },
    {
      id: 2,
      name: '微信原生小程序 + TDesign Mini Program',
      description: '微信小程序原生开发，使用TDesign组件库，提供高质量的UI组件和良好的用户体验。',
      category: 'frontend' as const,
      icon: '💬',
      tags: ['微信小程序', '原生开发', 'TDesign'],
    },
    {
      id: 3,
      name: 'Vue 3 + Vite + TDesign Vue Next',
      description: 'Web管理后台开发，使用Vue 3 + Vite构建工具，TDesign Vue Next组件库，快速开发企业级管理界面。',
      category: 'frontend' as const,
      icon: '🖥️',
      tags: ['Web管理后台', 'Vite', 'TDesign'],
    },
    // 后端技术
    {
      id: 4,
      name: 'ASP.NET Core Web API (net10.0)',
      description: '高性能、跨平台的Web API框架，基于.NET 10.0，提供RESTful API服务，支持微服务架构。',
      category: 'backend' as const,
      icon: '⚡',
      tags: ['Web API', '.NET 10', '高性能'],
    },
    {
      id: 5,
      name: '.NET 8.0 类库',
      description: '分层架构设计：YunQi.Applications（应用层）、YunQi.Domain（领域层）、YunQi.Infrastructure（基础设施层），实现高内聚低耦合。',
      category: 'backend' as const,
      icon: '🟣',
      tags: ['分层架构', '领域驱动设计', '.NET 8'],
    },
    {
      id: 6,
      name: 'EF Core + Npgsql (PostgreSQL)',
      description: 'Entity Framework Core ORM框架，使用Npgsql提供程序连接PostgreSQL数据库，支持Code First开发模式。',
      category: 'backend' as const,
      icon: '🗄️',
      tags: ['ORM', 'PostgreSQL', '数据库'],
    },
    // 其他技术
    {
      id: 7,
      name: 'Scalar / NSwag (API 文档)',
      description: 'API文档生成工具，Scalar提供现代化的API文档界面，NSwag自动生成客户端代码，提高开发效率。',
      category: 'other' as const,
      icon: '📖',
      tags: ['API文档', '代码生成', '开发工具'],
    },
    {
      id: 8,
      name: 'RSA-OAEP SHA-256 加密',
      description: '非对称加密算法，使用RSA-OAEP填充和SHA-256哈希，保障数据传输安全性，适用于敏感信息加密。',
      category: 'other' as const,
      icon: '🔒',
      tags: ['加密', '安全', 'RSA'],
    },
    {
      id: 9,
      name: 'PBKDF2 凭据校验',
      description: '基于密码的密钥派生函数，用于安全存储用户密码，通过加盐和多次迭代提高密码安全性。',
      category: 'other' as const,
      icon: '🛡️',
      tags: ['密码安全', '密钥派生', 'PBKDF2'],
    },
  ],

  // 技术分类
  techCategories: [
    { key: 'all', label: '全部' },
    { key: 'frontend', label: '前端' },
    { key: 'backend', label: '后端' },
    { key: 'other', label: '其他' },
  ],

  // 颜色配置
  colors: {
    primary: '#2ecc71',
    primaryDark: '#27ae60',
    secondary: '#1abc9c',
    background: '#e8f5e9',
    backgroundLight: '#f1f8e9',
    text: '#2c3e50',
    textLight: '#666',
    categoryFrontend: '#2ecc71',
    categoryBackend: '#3498db',
    categoryOther: '#f39c12',
  },

  // 项目经历
  projects: [
    {
      id: 1,
      name: '智慧园区管理平台',
      role: '全栈开发工程师',
      period: '2024.01 - 2024.06',
      company: '某科技公司',
      description: '园区智能化管理系统，涵盖设备监控、能耗管理、访客管理、停车管理等模块，实现园区数字化运营。',
      responsibilities: [
        '负责系统架构设计和核心模块开发',
        '使用 Vue 3 + TDesign Vue Next 开发管理后台',
        '使用 ASP.NET Core Web API 开发后端服务',
        '设计并实现 PostgreSQL 数据库结构',
      ],
      technologies: ['Vue 3', 'TypeScript', 'ASP.NET Core', 'PostgreSQL', 'EF Core'],
      achievements: [
        '系统上线后园区运营效率提升 30%',
        '能耗监控模块帮助园区节省电费 15%',
        '获得客户高度评价并续约二期项目',
      ],
      category: 'enterprise',
    },
    {
      id: 2,
      name: '移动端电商小程序',
      role: '前端开发工程师',
      period: '2023.06 - 2023.12',
      company: '某电商平台',
      description: '基于微信小程序的电商平台，支持商品浏览、购物车、订单管理、支付等功能，日活用户超过 10 万。',
      responsibilities: [
        '负责小程序前端架构设计和开发',
        '使用 uni-app 实现多端适配',
        '优化页面性能和用户体验',
        '与后端团队协作完成接口对接',
      ],
      technologies: ['uni-app', 'Vue 3', 'Pinia', '微信小程序', 'TDesign Mini Program'],
      achievements: [
        '首屏加载时间优化至 1.5 秒以内',
        '用户转化率提升 20%',
        '代码复用率达到 85%，显著降低维护成本',
      ],
      category: 'mobile',
    },
    {
      id: 3,
      name: '企业级微服务架构',
      role: '后端开发工程师',
      period: '2022.09 - 2023.05',
      company: '某互联网公司',
      description: '基于 .NET 8.0 的微服务架构系统，采用领域驱动设计（DDD），支持高并发和水平扩展。',
      responsibilities: [
        '负责微服务架构设计和实施',
        '实现领域驱动设计（DDD）分层架构',
        '设计并实现分布式缓存和消息队列',
        '编写单元测试和集成测试',
      ],
      technologies: ['.NET 8.0', 'EF Core', 'PostgreSQL', 'Redis', 'RabbitMQ'],
      achievements: [
        '系统支持 10000+ 并发请求',
        '服务可用性达到 99.9%',
        '获得团队技术分享一等奖',
      ],
      category: 'enterprise',
    },
    {
      id: 4,
      name: '数据可视化大屏',
      role: '全栈开发工程师',
      period: '2022.03 - 2022.08',
      company: '某数据公司',
      description: '实时数据可视化大屏系统，展示业务数据、运营指标、用户行为分析等，支持多维度数据筛选和钻取。',
      responsibilities: [
        '负责大屏前端开发和数据可视化实现',
        '使用 ECharts 实现多种图表类型',
        '开发 WebSocket 实时数据推送',
        '优化大屏在不同分辨率下的适配',
      ],
      technologies: ['Vue 3', 'ECharts', 'WebSocket', 'ASP.NET Core', 'PostgreSQL'],
      achievements: [
        '支持 4K 分辨率下的流畅展示',
        '实时数据延迟控制在 500ms 以内',
        '获得客户书面表扬信',
      ],
      category: 'visualization',
    },
    {
      id: 5,
      name: '安全认证中间件',
      role: '核心开发工程师',
      period: '2021.10 - 2022.02',
      company: '某安全公司',
      description: '企业级安全认证中间件，支持多种认证方式（OAuth2.0、JWT、SAML），提供统一的身份认证和授权服务。',
      responsibilities: [
        '设计并实现安全认证框架',
        '集成 RSA-OAEP SHA-256 加密算法',
        '实现 PBKDF2 密码安全存储',
        '编写安全审计日志模块',
      ],
      technologies: ['ASP.NET Core', 'OAuth2.0', 'JWT', 'RSA', 'PBKDF2'],
      achievements: [
        '通过等保三级安全认证',
        '支持 10+ 种认证方式',
        '被 5 个企业项目采用',
      ],
      category: 'security',
    },
  ],

  // 项目分类
  projectCategories: [
    { key: 'all', label: '全部项目' },
    { key: 'enterprise', label: '企业应用' },
    { key: 'mobile', label: '移动端' },
    { key: 'visualization', label: '数据可视化' },
    { key: 'security', label: '安全相关' },
  ],

  // 简历模块标题
  projectsTitle: '项目经历',
  projectsSubtitle: '我参与过的代表性项目',
}

export type SiteConfig = typeof siteConfig
