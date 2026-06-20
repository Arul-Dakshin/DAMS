import { Component, input } from '@angular/core';

/**
 * Standard page heading with an optional subtitle and a projected actions slot
 * (e.g. a "Create" button), so every feature page has a consistent header.
 */
@Component({
  selector: 'app-page-header',
  template: `
    <div class="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
      <div>
        <h4 class="fw-bold mb-0">{{ title() }}</h4>
        @if (subtitle()) {
          <small class="text-muted">{{ subtitle() }}</small>
        }
      </div>
      <ng-content />
    </div>
  `
})
export class PageHeader {
  readonly title = input.required<string>();
  readonly subtitle = input<string>();
}
