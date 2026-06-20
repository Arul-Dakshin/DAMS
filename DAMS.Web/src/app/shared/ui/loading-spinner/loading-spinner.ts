import { Component, input } from '@angular/core';

/** Centered loading indicator used while a page is fetching data. */
@Component({
  selector: 'app-loading-spinner',
  template: `
    <div class="text-center text-muted py-5">
      <div class="spinner-border text-primary mb-2" role="status">
        <span class="visually-hidden">Loading</span>
      </div>
      <div>{{ message() }}</div>
    </div>
  `
})
export class LoadingSpinner {
  readonly message = input('Loading…');
}
