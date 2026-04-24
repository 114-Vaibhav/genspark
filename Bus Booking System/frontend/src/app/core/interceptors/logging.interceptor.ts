import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { tap } from 'rxjs';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
  console.log('[HTTP] Request started', {
    method: req.method,
    url: req.urlWithParams,
    body: req.body
  });

  return next(req).pipe(
    tap({
      next: (event) => {
        console.log('[HTTP] Request succeeded', {
          url: req.urlWithParams,
          event
        });
      },
      error: (error: HttpErrorResponse) => {
        console.error('[HTTP] Request failed', {
          url: req.urlWithParams,
          status: error.status,
          message: error.message,
          error: error.error
        });
      }
    })
  );
};
