import { FurnitureAdditionalPhoto, FurnitureReview } from "./furniture.review.model";

export interface itemGet
{
    id: number;
    name: string;
    description: string;
    price: number;
    category: string;
    color: string;
    image: string;
    manufacturer: string;
    master: string;
    reviews: FurnitureReview[];
    additionalPhotos: FurnitureAdditionalPhoto[];
    averageRating: number;
    reviewCount: number;
}