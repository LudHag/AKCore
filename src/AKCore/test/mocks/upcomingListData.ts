/** Display name used in mock events and tests that assert on rendered text. */
export const mockUpcomingEventName = "Component Test Gig";

/** Minimal payload matching `/Upcoming/UpcomingListData` JSON shape used by UpcomingApp. */
export function createUpcomingListDataPayload(loggedIn: boolean) {
  return {
    years: {
      "2026": {
        year: 2026,
        months: {
          "5": [
            {
              id: 4242,
              type: "Spelning",
              name: mockUpcomingEventName,
              place: "Test Venue",
              description: "",
              descriptionEng: "",
              internalDescription: "",
              internalDescriptionEng: "",
              fikaCollection: [] as string[],
              year: 2026,
              month: 5,
              day: "monday 05/05",
              dayDate: "2026-05-05",
              dayInMonth: 5,
              halanTime: "",
              thereTime: "",
              startsTime: "19:00",
              playDuration: "",
              stand: "",
              secret: false,
              signupState: null,
              coming: 0,
              notComing: 0,
              disabled: false,
            },
          ],
        },
      },
    },
    loggedIn,
    member: false,
    icalLink: "https://example.test/upcoming/akevents.ics",
  };
}
