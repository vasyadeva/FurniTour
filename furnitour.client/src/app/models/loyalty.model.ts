export interface LoyaltyModel {
  loyaltyLevel: number;
  loyaltyName: string;
  discountPercent: number;
  totalSpent: number;
  nextLevelThreshold: number;
  amountToNextLevel: number;
}
