export interface NamedLink {
  name: string
  url: string
}

export interface NavLink {
  name: string
  href: string
}

export interface SiteStat {
  value: string
  label: string
}

export interface SitePersonal {
  name: string
  title: string
  avatarText: string
  bio: string
}

export interface SiteContact {
  email: string
  location: string
  status: string
}

export interface CategoryOption {
  key: string
  label: string
}

export interface SiteColors {
  primary: string
  primaryDark: string
  secondary: string
  background: string
  backgroundLight: string
  text: string
  textLight: string
  categoryFrontend: string
  categoryBackend: string
  categoryOther: string
}

export interface TechStackItem {
  id: number
  name: string
  description: string
  category: string
  icon: string
  tags: string[]
}

export interface ProjectItem {
  id: number
  name: string
  role: string
  period: string
  company: string
  description: string
  responsibilities: string[]
  technologies: string[]
  achievements: string[]
  category: string
}

export interface SiteConfig {
  personal: SitePersonal
  stats: SiteStat[]
  socialLinks: NamedLink[]
  contact: SiteContact
  navLinks: NavLink[]
  pageTitle: string
  skillsTitle: string
  skillsSubtitle: string
  contactTitle: string
  footerText: string
  techStack: TechStackItem[]
  techCategories: CategoryOption[]
  colors: SiteColors
  projects: ProjectItem[]
  projectCategories: CategoryOption[]
  projectsTitle: string
  projectsSubtitle: string
}

export interface SiteConfigResponse {
  techStack?: TechStackItem[] | null
  projects?: ProjectItem[] | null
}
