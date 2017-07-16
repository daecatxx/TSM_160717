<template>
  <v-toolbar class="brown lighten-3" light>
    <v-toolbar-side-icon light></v-toolbar-side-icon>
    <v-toolbar-title>Toolbar</v-toolbar-title>
    <v-toolbar-items>
      <v-menu origin="right center" transition="v-scale-transition" left>
        <v-btn light icon slot="activator">
          <v-icon>more_vert</v-icon>
        </v-btn>
        <v-list>
          <v-list-item>
            <v-list-tile router action="link" href="/view_past_month_timesheets">
              <v-list-tile-title>Previous month timesheet</v-list-tile-title>
            </v-list-tile>
            <v-list-tile router action="link" href="/view_current_month_timesheet">
  
              <v-list-tile-title>Current month timesheet</v-list-tile-title>
  
            </v-list-tile>
            <v-list-tile router action="link" href="/generate_this_month_timesheet">
              <v-list-tile-title>Generate this month timesheet</v-list-tile-title>
            </v-list-tile>
  
  
            <!--<v-list-tile asp-area="" asp-controller="Account" asp-action="Logout">-->
            <!--<a asp-area="" asp-controller="Account" asp-action="Logout">
            <form asp-area="" asp-controller="Account" asp-action="Logout" method="post"></form>-->
              <v-list-tile id="logOutBtn" @@click.prevent="logOut"> 
                <!--<v-btn></v-btn>-->
                <!--router asp-controller="Account" asp-action="Logout" method="post">-->
              <v-list-tile-title>Log Out</v-list-tile-title>
              </v-list-tile>
            
          </v-list-item>
        </v-list>
      </v-menu>
    </v-toolbar-items>
  </v-toolbar>
</template>
<script>
export default {
  name: 'Top',
  methods:
  {
    logOut:function() {
      console.log("logging out");
      $logOutHandler = jQuery.ajax({
                        type: 'POST',
                        url: '../../../../Account/Logout',

    });
  }

}
}

</script>

<style scoped>
header {
  border-bottom: 1px solid #42b983;
  margin-bottom: 30px;
  height: 10vh;
}
</style>