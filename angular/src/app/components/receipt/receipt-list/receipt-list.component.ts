import { Component, OnInit } from '@angular/core';
import { ReceiptService } from '../../../shared/services/receipt.service';
import { Receipt } from '../../../shared/models/receipt';
import { Router } from '@angular/router';
import {NgForOf} from '@angular/common';
import {Customer} from '../../../shared/models/customer';
import {FormatDatePipe} from '../../../shared/pipes/format-date.pipe';
import {SortPipe} from '../../../shared/pipes/sort.pipe';

@Component({
  selector: 'app-receipt-list',
  templateUrl: './receipt-list.component.html',
  imports: [
    NgForOf,
    FormatDatePipe,
    SortPipe
  ],
  styleUrls: ['./receipt-list.component.css']
})
export class ReceiptListComponent implements OnInit  {
  receipts: Receipt[] = [];
  paginatedReceipts: Receipt[] = [];

  sortField: string = '';
  sortOrder: 'asc' | 'desc' = 'asc';

  currentPage: number = 1;
  itemsPerPage: number = 5;
  totalPages: number = 1;

  constructor(
    private receiptService: ReceiptService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadReceipts();
  }

  toggleSort(field: string): void {
    if (this.sortField === field) {
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortOrder = 'asc';
    }
  }

  getSortIcon(field: string): string {
    if (this.sortField === field) {
      return this.sortOrder === 'asc'
        ? '<i class="bi bi-sort-alpha-up"></i>'
        : '<i class="bi bi-sort-alpha-down"></i>';
    }
    return '<i class="bi bi-sort"></i>';
  }

  updatePagination(): void {
    this.totalPages = Math.ceil(this.receipts.length / this.itemsPerPage);
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.paginatedReceipts = this.receipts.slice(startIndex, endIndex);
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagination();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagination();
    }
  }

  loadReceipts(): void {
    this.receiptService.getAllReceipts().subscribe(
      (data) => {
        this.receipts = data;
        this.updatePagination();
      },
      (error) => {
        console.error('Failed to load receipts:', error);
      }
    );
  }

  editReceipt(id: number): void {
    this.router.navigate([`/receipts/edit/${id}`]);
  }

  deleteReceipt(id: number): void {
    if (confirm('Are you sure you want to delete this receipt?')) {
      this.receiptService.deleteReceipt(id).subscribe(
        () => {
          console.log('Receipt deleted successfully');
          this.loadReceipts();
          this.updatePagination();
        },
        (error) => {
          console.error('Failed to delete receipt:', error);
        }
      );
    }
  }

  addReceipt(): void {
    this.router.navigate(['/receipts/add']);
  }
}
