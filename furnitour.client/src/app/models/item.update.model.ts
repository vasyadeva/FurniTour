export interface itemUpdate
{
    id: number
    name: string;
    description: string;
    price: number;
    image: string;
    categoryId: number;
    colorId: number;
    manufacturerId: number;
    additionalPhotos: string[]; // Base64 encoded images
    photoDescriptions: string[];
    photoIdsToRemove: number[]; // IDs of photos to be removed
}