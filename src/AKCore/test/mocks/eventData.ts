import type {
  UpcomingEvent,
  UpcomingEventInfo,
} from "@components/Upcoming/models";

export const mockEventDetailName = "Component Test Event Detail";

export const mockEventDetailId = 5151;

function createMockEvent(id: number): UpcomingEvent {
  return {
    id,
    type: "Spelning",
    name: mockEventDetailName,
    place: "Test Venue",
    description: "",
    descriptionEng: "",
    internalDescription: "",
    internalDescriptionEng: "",
    fikaCollection: [],
    year: 2026,
    month: 5,
    day: "monday 05/05",
    dayDate: "2026-05-05",
    dayInMonth: 5,
    halanTime: "18:00",
    thereTime: "",
    startsTime: "19:00",
    playDuration: "",
    stand: "",
    secret: false,
    signupState: null,
    coming: 0,
    notComing: 0,
    disabled: false,
  };
}

export function createEventDataPayload(
  overrides: Partial<UpcomingEventInfo> = {},
): UpcomingEventInfo {
  const event = createMockEvent(mockEventDetailId);
  return {
    where: null,
    car: false,
    instrument: true,
    comment: "",
    isNintendo: false,
    isPassed: false,
    members: [],
    event,
    signups: [],
    availableInstruments: ["Flute"],
    selectedInstrument: "Flute",
    ...overrides,
  };
}
