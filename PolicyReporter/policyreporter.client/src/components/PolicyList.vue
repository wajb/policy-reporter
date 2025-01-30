<template>
  <div class="loading-panel" :style="{ opacity: isLoading ? 1 : 0 }">
    <p>Loading...</p>
  </div>
  <section>
    <h2>Summary</h2>
    <p>Policy count: {{ report?.statistics.policyCount ?? 0 }}</p>
    <p>Customer count: {{ report?.statistics.customerCount ?? 0 }}</p>
    <p>Insured amount total: {{ report?.statistics.insuredAmountTotal ?? 0 }}</p>
    <p>Average policy duration (in days): {{ report?.statistics.averagePolicyDurationDays ?? 0 }}</p>
  </section>
  <table>
    <thead>
      <tr>
        <th v-for="columnName in policyColumns"
          @click="sortBy(columnName)"
          :class="{ active: sortColumnName == columnName }"
          :key="columnName">
          {{ columnName }}
          <span :class="'arrow ' + (sortOrders[columnName] == 1 ? 'asc' : 'desc')">
          </span>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="policy in sortedPolicies" :key="policy.policyId">
        <td v-for="columnName in policyColumns" :key="columnName">
          {{policy[columnName]}}
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script lang="ts">
  import { defineComponent } from 'vue';

  type Policy = {
    policyId: string;
    sourceId: number;
    insuredAmount: number;
    currency: string;
    customer: string;
    startDate?: Date;
    endDate?: Date;
  };

  type PolicyColumnName = keyof Policy;

  interface PolicyStatistics {
    policyCount: number;
    customerCount: number;
    insuredAmountTotal: number;
    averagePolicyDurationDays: number;
  }

  interface Report {
    statistics: PolicyStatistics;
    policies: (Policy & { _id: string })[];
  }

  interface Data {
    isLoading: boolean;
    policyColumns: PolicyColumnName[];
    sortOrders: { [columnName in PolicyColumnName]: 1 | -1 };
    sortColumnName?: keyof Policy;
    report: Report | null;
  }

  export default defineComponent({
    props: {
      brokerFilter: [String, null],
      activeOnlyFilter: Boolean,
    },
    data(): Data {
      const policyColumns: PolicyColumnName[] = [
        'policyId',
        'sourceId',
        'insuredAmount',
        'currency',
        'customer',
        'startDate',
        'endDate',
      ];
      const sortOrders = policyColumns.reduce((result, columnName) => ({ ...result, [columnName]: 1 }),
        {} as { [columnName in PolicyColumnName]: 1 | -1 });

      return {
        isLoading: false,
        policyColumns,
        sortOrders,
        report: null,
        sortColumnName: 'policyId',
      };
    },
    computed: {
      sortedPolicies(): Report['policies'] | undefined {
        // TODO sorting
        return this.report?.policies;
      }
    },
    async created() {
      await this.fetchData();
    },
    methods: {
      async fetchData() {
        this.report = null;
        this.isLoading = true;

        const queryParams = {
          broker: this.brokerFilter || '',
          activeOnly: this.activeOnlyFilter?.toString(),
        };

        const response = await fetch('policyreport?' + new URLSearchParams(queryParams).toString());
        if (response.ok) {
            this.report = await response.json();
            this.isLoading = false;
        }
      },
      sortBy(columnName: PolicyColumnName) {
        this.sortColumnName = columnName;
        this.sortOrders[columnName] *= -1;
      }
    },
  });
</script>

<style scoped>
.loading-panel {
  position: absolute;
  left: 0;
  right: 0;
  height: 100vh;
  margin: 0 auto;
  background-color: rgba(255, 255, 255, 0.66);
  transition: opacity 0.5s;
  pointer-events: none;
}

.loading-panel p {
  height: 1em;
  margin: 50vh auto 0 auto;
  text-align: center;
  font-weight: bold;
}

section, table {
  border: 2px solid #42b983;
  border-radius: 3px;
  background-color: #fff;
  width: 100%;
}

section {
  margin-bottom: 1em;
  padding: 1em;
}

th {
  background-color: #42b983;
  color: rgba(255, 255, 255, 0.66);
  cursor: pointer;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
}

td {
  background-color: #f9f9f9;
}

th,
td {
  min-width: 120px;
  padding: 10px 20px;
}

th.active {
  color: #fff;
}

th.active .arrow {
  opacity: 1;
}

.arrow {
  display: inline-block;
  vertical-align: middle;
  width: 0;
  height: 0;
  margin-left: 5px;
  opacity: 0.66;
}

.arrow.asc {
  border-left: 4px solid transparent;
  border-right: 4px solid transparent;
  border-bottom: 4px solid #fff;
}

.arrow.desc {
  border-left: 4px solid transparent;
  border-right: 4px solid transparent;
  border-top: 4px solid #fff;
}
</style>
