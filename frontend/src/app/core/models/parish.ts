export interface Parish { id: string; name: string; sector: string; city: string; state: string; address: string; phone?: string; isPremium: boolean; lastScheduleConfirmation: string; massSchedules: MassSchedule[]; activities: Activity[]; chapels: Chapel[]; }
export interface MassSchedule { day: string; time: string; description?: string; }
export interface Activity { title: string; startsAt: string; description?: string; }
export interface Chapel { name: string; address?: string; }
