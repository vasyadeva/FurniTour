export interface itemSend
{
    name: string;
    description: string;
    price: number;
    image: string;
    categoryId: number;
    colorId: number;
    manufacturerId: number;
    additionalPhotos: string[]; // Base64 encoded images
    photoDescriptions: string[];
}