import { Component, input } from '@angular/core';

/** Friendly placeholder shown when a list has no records. */
@Component({
  selector: 'app-empty-state',
  template: `
    <div class="text-center text-muted py-5">
      <i class="bi {{ icon() }} fs-1 d-block mb-2 opacity-50"></i>
      <div>{{ message() }}</div>
    </div>
  `
})
export class EmptyState {
  readonly icon = input('bi-inbox');
  readonly message = input('Nothing here yet.');
}
