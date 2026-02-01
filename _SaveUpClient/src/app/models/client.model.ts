export interface Client {
  id: number;
  creditScore: number;
  segment: Segment;
  riskLevel: number;
}

export interface Segment {
  id: number;
  name: string;
}