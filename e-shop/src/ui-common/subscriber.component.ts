import { Component, OnDestroy } from "@angular/core";
import { Subject } from "rxjs";

@Component({
    template: '',
    styles: [],
})
export abstract class SubscriberComponent implements OnDestroy {
    protected readonly ngUnsubscribe: Subject<void> = new Subject<void>();

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
}
