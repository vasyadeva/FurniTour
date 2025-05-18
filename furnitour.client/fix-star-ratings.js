// This script finds and replaces the star rating display code in items.component.html
// Run with: node fix-star-ratings.js

const fs = require('fs');
const path = require('path');

// Path to the items.component.html file
const htmlFilePath = path.join(__dirname, 'src', 'app', 'item', 'items', 'items.component.html');

// Read the file
let htmlContent = fs.readFileSync(htmlFilePath, 'utf8');

// Old pattern to find 
const oldPattern = `<div *ngIf="item.reviewCount > 0" class="d-flex justify-content-center align-items-center mb-2">
                <span *ngFor="let i of [1,2,3,4,5].slice(0, math.floor(item.averageRating))" class="text-warning">
                  <i class="fas fa-star"></i>
                </span>
                <span *ngIf="item.averageRating % 1 > 0" class="text-warning">
                  <i class="fas fa-star-half-alt"></i>
                </span>
                <span *ngFor="let i of [1,2,3,4,5].slice(0, 5 - math.ceil(item.averageRating))" class="text-muted">
                  <i class="fas fa-star"></i>
                </span>
                <span class="ms-2 text-muted small">({{ item.reviewCount }})</span>
              </div>`;

// New replacement
const newPattern = `<div *ngIf="item.reviewCount > 0" class="d-flex justify-content-center align-items-center mb-2">
                <span *ngFor="let star of getFullStars(item.averageRating)" class="text-warning">
                  <i class="fas fa-star"></i>
                </span>
                <span *ngIf="getHalfStar(item.averageRating)" class="text-warning">
                  <i class="fas fa-star-half-alt"></i>
                </span>
                <span *ngFor="let star of getEmptyStars(item.averageRating)" class="text-muted">
                  <i class="fas fa-star"></i>
                </span>
                <span class="ms-2 text-muted small">({{ item.reviewCount }})</span>
              </div>`;

// Replace all occurrences
while (htmlContent.includes(oldPattern)) {
    htmlContent = htmlContent.replace(oldPattern, newPattern);
}

// Write the updated content back to the file
fs.writeFileSync(htmlFilePath, htmlContent);

console.log('Star rating displays have been updated in items.component.html');
