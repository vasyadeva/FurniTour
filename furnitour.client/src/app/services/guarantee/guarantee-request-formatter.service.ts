import { Injectable } from '@angular/core';
import { GuaranteeAddModel } from '../../models/guarantee.model';

@Injectable({
  providedIn: 'root'
})
export class GuaranteeRequestFormatterService {
  /**
   * Format a guarantee request by ensuring all items are valid integers
   * @param request The original guarantee request data
   * @returns Formatted guarantee request with validated items
   */  formatGuaranteeRequest(request: GuaranteeAddModel): GuaranteeAddModel {
    if (!request) {
      return request;
    }
    
    console.log('Original guarantee request:', JSON.stringify(request));
    
    // Create a copy of the request to avoid mutating the original
    const formattedRequest: GuaranteeAddModel = {
      orderId: request.orderId,
      comment: request.comment,
      photos: request.photos || [],
      items: []
    };
    
    // Filter and convert items to valid integers
    if (request.items && Array.isArray(request.items)) {
      console.log('Original items array:', JSON.stringify(request.items));
      console.log('Items types:', request.items.map(item => typeof item));
      
      // Додаткова перевірка на обов'язкову наявність елементів
      if (request.items.length === 0) {
        console.error('Empty items array detected. This will cause validation errors.');
      }
      
      formattedRequest.items = request.items
        .filter(item => item !== null && item !== undefined)
        .map(item => {
          const parsed = typeof item === 'string' ? parseInt(item, 10) : Number(item);
          console.log(`Converting item ${item} (${typeof item}) to ${parsed} (${typeof parsed})`);
          return parsed;
        })
        .filter(item => !isNaN(item) && item > 0);
      
      console.log('Formatted items array:', JSON.stringify(formattedRequest.items));
      
      // Додаткова перевірка після фільтрації
      if (formattedRequest.items.length === 0) {
        console.error('All items were filtered out. This will cause validation errors.');
        throw new Error('No valid items found to submit. Please try selecting different items.');
      }
    } else {
      console.warn('Items is not an array or is null/undefined:', request.items);
      throw new Error('Invalid items format. Please try again with valid selection.');
    }
    
    return formattedRequest;
  }
}
