import { InjectionToken } from '@Angular/core';

export const lookupSlotToken = new InjectionToken('lookupSlotToken');

export const lookupSlot = {
  options: ['All', 'Booked']
};
