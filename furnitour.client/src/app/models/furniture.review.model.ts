export interface FurnitureReview {
    id: number;
    comment: string;
    rating: number;
    username: string;
    createdAt: Date;
}

export interface AddFurnitureReview {
    furnitureId: number;
    comment: string;
    rating: number;
}

export interface FurnitureAdditionalPhoto {
    id: number;
    photoData: string;
    description: string;
}
